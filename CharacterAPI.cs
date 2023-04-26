using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace CharacterAPI
{
    public struct CharacterAPI
    {
        public static Sprite[] GetFilesForAnimation(int amountOfSprites, string Path_Before_File_Name)
        {
            Sprite[] files = new Sprite[amountOfSprites];

            for (int i = 0; i < amountOfSprites; i++)
            {
                files[i] = ModAPI.LoadSprite(Path_Before_File_Name + i + ".png");
            }

            return files;
        }
    }

    public class Character
    {
        public string name;
        public string description;
        public string ThumnbnailOverride;
        public string ItemToOverride;

        public Action<GameObject> AfterSpawn;

        public GameObject player;

        public Rigidbody2D rb;

        public SpriteRenderer spriteRenderer;

        public Collider2D collider;

        int animationNumber = 0;

        public float xScale = 2, yScale = 2;
        public float colliderWidth, colliderHeight;
        public float mass = 1;

        float time;

        public float acceleration = 100;
        public float maxSpeed = 7;
        public float friction = 0.99f;

        public float jumpForce = 10;
        public float idleGravity = 1;
        public float fallingGravity = 2;

        public void SimpleMovement(float axisOfMovement, bool flipFromInput, bool useFriction)
        {
            rb.AddForce(player.transform.right * axisOfMovement * acceleration);

            if (flipFromInput)
            {
                if(axisOfMovement < 0)
                {
                    player.transform.localScale = new Vector2(-xScale, yScale);
                }
                if(axisOfMovement > 0)
                {
                    player.transform.localScale = new Vector2(xScale, yScale);
                }
            }

            if (!useFriction)
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x * friction, -maxSpeed, maxSpeed), rb.velocity.y);
        }

        public void SimpleJump()
        {
            rb.AddForce(player.transform.up * jumpForce, ForceMode2D.Impulse);
        }

        public bool RayOnGround(float range)
        {
            return Physics2D.Raycast(player.transform.position, -player.transform.up, range);
        }

        public bool IsRayTrue(Vector2 position, Vector2 direction, float distance)
        {
            return Physics2D.Raycast(position, direction, distance);
        }

        public void AnimateSpriteWithArray(Sprite[] spritesToUse, float timeInBetweenSprites, int spriteToResetTo, SpriteRenderer rendToUse)
        {
            time -= Time.deltaTime;

            if(time <= 0)
            {
                animationNumber++;
                time = timeInBetweenSprites;
            }

            if(animationNumber > spritesToUse.Length - 1)
            {
                animationNumber = spriteToResetTo;
            }

            
            player.GetComponent<PhysicalBehaviour>().RefreshOutline();
            rendToUse.sprite = spritesToUse[animationNumber];
        }

        public void SetSprite(Sprite sprite, SpriteRenderer rend)
        {
            rend.sprite = sprite;
            player.GetComponent<PhysicalBehaviour>().RefreshOutline();
        }

        public void ApplyForce(float force, Vector2 direction, ForceMode2D modeOfForce)
        {
            rb.AddForce(direction * force, modeOfForce);
        }

        public void LockRotation()
        {
            rb.freezeRotation = true;
        }

        public void ChangePhysicalMaterial(float Bounciness, float Friction)
        {
            PhysicsMaterial2D mat = new PhysicsMaterial2D();

            mat.friction = Friction;
            mat.bounciness= Bounciness;

            rb.sharedMaterial = mat;
        }
    }
}
