using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Microsoft.Xna.Framework;
using Nez.Sprites;
using Nez.Tiled;

namespace ScreenWrap
{
    public class PlayerController : Component, IUpdatable
    {
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState = new TiledMapMover.CollisionState();
        BoxCollider collider;
        Sprite sprite;

        float moveSpeed = 120f;
        float airMoveSpeed = 120f;
        Vector2 velocity;
        float gravity = 800f;
        float maxSpeed => moveSpeed * 3f;
        Vector2 maxSpeedVec;

        float jumpHeight = 16 * 3 + 4; //16px tiles * tiles high + buffer
        bool canJumpThisFrame
        {
            get { return collisionState.below || (offGroundInputBufferTimer > 0 && justJumpedBufferTimer <= 0); }
        }
        int offGroundInputBufferFrames = 8;
        int offGroundInputBufferTimer = 0;
        int landingInputBufferFrames = 4;
        int landingInputBufferTimer = 0;
        int justJumpedBufferTimer = 0;

        Input input;
        public override void onAddedToEntity()
        {
            input = entity.getComponent<Input>();
            mover = entity.getComponent<TiledMapMover>();
            sprite = entity.getComponent<Sprite>();
            collider = entity.getComponents<BoxCollider>().FirstOrDefault(c => c.name == "hurtbox");
            velocity = Vector2.Zero;
            maxSpeedVec = new Vector2(maxSpeed, maxSpeed);
        }

        public void update()
        {
            move();
            if (velocity.X > 0)
            {
                sprite.flipX = false;
            }
            else if (velocity.X < 0) sprite.flipX = true;
        }



        public void move()
        {
            //groundInputBuffer
            if (collisionState.wasGroundedLastFrame && !collisionState.below)
            {
                offGroundInputBufferTimer = offGroundInputBufferFrames;
            }

            //handle left/right
            if (collisionState.below) velocity.X = input.onlyXInput.X * moveSpeed;
            else velocity.X = input.onlyXInput.X * airMoveSpeed;

            //drop through one way
            if (collisionState.isGroundedOnOneWayPlatform && input.onlyYInput.Y > 0 && input.Button2Input.isPressed)
            {
                entity.setPosition(new Vector2(entity.position.X, entity.position.Y + 1));
                collisionState.clearLastGroundTile();
                landingInputBufferTimer = 0;
            }
            //jump
            else if (input.Button2Input.isPressed)
            {
                if (canJumpThisFrame) Jump();
                else landingInputBufferTimer = landingInputBufferFrames;
            }
            // jump if you recently hit jump before you landed
            else if (landingInputBufferTimer > 0)
            {
                landingInputBufferTimer--;
                if (collisionState.below) Jump();
            }
            // handle variable jump height
            if (!collisionState.below && input.Button2Input.isReleased)
            {
                velocity.Y *= 0.5f;
            }

            //apply gravity
            velocity.Y += gravity * Time.deltaTime;

            velocity = Vector2.Clamp(velocity, -maxSpeedVec, maxSpeedVec);

            var oldPos = entity.position;
            mover.move(velocity * Time.deltaTime, collider, collisionState);

            var leftSide = entity.position.X - collider.width / 2;
            var rightSide = entity.position.X + collider.width / 2;
            
            // dont let gravity just build while you're grounded
            if (collisionState.above || collisionState.below)
                velocity.Y = 0;
            // tick jump input buffer timer
            if (offGroundInputBufferTimer > 0)
                offGroundInputBufferTimer--;
            if (justJumpedBufferTimer > 0)
                justJumpedBufferTimer--;
        }

        void Jump()
        {
            velocity.Y = -Mathf.sqrt(2 * jumpHeight * gravity);
            landingInputBufferTimer = 0;
            justJumpedBufferTimer = offGroundInputBufferFrames;
        }
    }
}
