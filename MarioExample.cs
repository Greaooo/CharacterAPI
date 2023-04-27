    public class Mod : MonoBehaviour
    {
        public static void OnLoad()
        {
            ModAPI.RegisterInput("a", "a", KeyCode.A);
            ModAPI.RegisterInput("d", "d", KeyCode.D);
            ModAPI.RegisterInput("w", "w", KeyCode.W);
        }

        public static void Main()
        {
            ModAPI.Register(new Modification()
            {
                OriginalItem = ModAPI.FindSpawnable("Metal Cube"),
                NameOverride = "Mario",
                DescriptionOverride = "Mario, from mario",
                CategoryOverride = ModAPI.FindCategory("Entities"),
                ThumbnailOverride = ModAPI.LoadSprite("sprites/mario/idle.png"),

                AfterSpawn = (instance) =>
                {
                    MarioController script = instance.AddComponent<MarioController>();

                    Character mario = new Character()
                    {
                        player = instance,

                        //acceleration is how fast you want the character to hit its max speed
                        //maxspeed is what the rigidbody velocity is capped to
                        //friction is what the x velocity is multiplied by to lower it. if this is above 1 the character will infinitely accelerate
                        acceleration = 200,
                        maxSpeed = 7,
                        friction = 0.95f,

                        jumpForce = 25,

                        //idle gravity: The amount of gravity you want when just standing or not being changed by script
                        //falling gravity: The amount of  gravity you want when falling or letting go of the jump button to make your character more snappy.
                        idleGravity = 2,
                        fallingGravity = 4,

                        rb = instance.GetComponent<Rigidbody2D>(),

                        //scale of the gameobject of character
                        xScale = 2f,
                        yScale = 2f,

                        //the x and y scale of this collider
                        colliderX = 0.5f,
                        colliderY = 0.5f,

                        mass = 5,
                    };

                    script.character = mario;

                    script.character.ScaleBoxCollider(instance.GetComponent<BoxCollider2D>());

                    script.character.LockRotation();
                    script.character.ChangePhysicalMaterial(Friction: 0.6f, Bounciness: 0);
                    script.character.SetSprite(ModAPI.LoadSprite("sprites/mario/idle.png"), instance.GetComponent<SpriteRenderer>());

                }
            });
        }
    }

    public class MarioController : MonoBehaviour
    {
        public Character character;

        float inputDirection;

        Sprite idle;
        Sprite[] walkingSprites;

        void Awake()
        {
            //get idle sprite
            idle = ModAPI.LoadSprite("sprites/mario/idle.png");

            //get the running sprites in an array for later.
            walkingSprites = CharacterAPI.GetFilesForAnimation(3, "sprites/mario/run");
        }

        void FixedUpdate()
        {
            //simple movement
            character.SimpleMovement(input: inputDirection, flipFromInput: true, useFriction: true);
        }

        void Update()
        {
            //getting an input axis for movement
            if (InputSystem.Held("a"))
            {
                inputDirection = -1;
            }
            if (InputSystem.Held("d"))
            {
                inputDirection = 1;
            }
            if(!InputSystem.Held("a") && !InputSystem.Held("d"))
            {
                inputDirection = 0;
            }

            if(inputDirection != 0)
            {
                character.AnimateSpriteWithArray(spritesToUse: walkingSprites, timeInBetweenSprites: 0.1f, spriteToResetTo: 0, rendToUse: GetComponent<SpriteRenderer>());
            }
            else
            {
                character.SetSprite(idle, GetComponent<SpriteRenderer>());
            }

            //test if player is on ground by using ray, and if on ground and pressing w, jump.
            if(character.RayOnGround(range: 0.6f) && InputSystem.Down("w")) 
            {
                character.SimpleJump();
            }
        }
    }
