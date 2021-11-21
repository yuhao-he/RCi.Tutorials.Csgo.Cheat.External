using System;
using System.Diagnostics;
using System.Media;
using System.Threading;
using RCi.Tutorials.Csgo.Cheat.External.Data;
using RCi.Tutorials.Csgo.Cheat.External.Data.Internal;
using RCi.Tutorials.Csgo.Cheat.External.Gfx.Math;
using RCi.Tutorials.Csgo.Cheat.External.Sys.Data;
using RCi.Tutorials.Csgo.Cheat.External.Utils;

namespace RCi.Tutorials.Csgo.Cheat.External.Features
{
    /// <summary>
    /// Trigger bot. Shoots when hovering over an enemy.
    /// </summary>
    public class Sonar :
        ThreadedComponent
    {
        #region // storage

        /// <inheritdoc />
        protected override string ThreadName => nameof(Sonar);

        /// <inheritdoc cref="GameProcess"/>
        private GameProcess GameProcess { get; set; }

        /// <inheritdoc cref="GameData"/>
        private GameData GameData { get; set; }

        /// <inheritdoc cref="GameConsole"/>
        private GameConsole GameConsole { get; set; }

        private bool toggle = true;
        #endregion

        #region // ctor

        /// <summary />
        public Sonar(GameProcess gameProcess, GameData gameData, GameConsole gc)
        {
            GameProcess = gameProcess;
            GameData = gameData;
            GameConsole = gc;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();

            GameData = default;
            GameProcess = default;
        }

        #endregion

        #region // routines


        /// <inheritdoc />
        protected override void FrameAction()
        {
            if (!GameProcess.IsValid)
            {
                return;
            }

            if (!GameData.UseSonar) {
                return;
            }

            if (WindowsVirtualKey.VK_NUMPAD1.IsKeyDown())
            {
                Console.Beep();
                toggle = !toggle;
            }
            else if (WindowsVirtualKey.VK_NUMPAD2.IsKeyDown()) {
                toggle = !toggle;
            }
            if (!toggle) {
                return;
            }

            // get aim ray in world
            var player = GameData.Player;
            if (player.AimDirection.Length() < 0.001)
            {
                return;
            }
            var aimRayWorld = new Line3D(player.EyePosition, player.EyePosition + player.AimDirection * 8192);

            // go through entities
            foreach (var entity in GameData.Entities)
            {
                if (!entity.IsAlive() || entity.AddressBase == player.AddressBase || entity.Team == GameData.Player.Team)
                {
                    continue;
                }
                // check if aim ray intersects any hitboxes of entity
                var hitBoxId = IntersectsHitBox(aimRayWorld, entity);
                if (hitBoxId >= 0)
                {
                    // TODO: Play music
                    // Program.GameConsole.SendCommand(@"play ui\beep07;");
                    // GameConsole.SendCommand("play ui/beep07;");
                    System.Console.Beep();
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Check if aim ray intersects any hitbox of entity.
        /// </summary>
        /// <returns>
        /// Returns id of intersected hitbox, otherwise -1.
        /// </returns>
        public static int IntersectsHitBox(Line3D aimRayWorld, Entity entity)
        {
            for (var hitBoxId = 0; hitBoxId < entity.StudioHitBoxSet.numhitboxes; hitBoxId++)
            {
                var hitBox = entity.StudioHitBoxes[hitBoxId];
                var boneId = hitBox.bone;
                if (boneId < 0 || boneId > Offsets.MAXSTUDIOBONES || hitBox.radius <= 0)
                {
                    continue;
                }

                // intersect capsule
                var matrixBoneModelToWorld = entity.BonesMatrices[boneId];
                var boneStartWorld = matrixBoneModelToWorld.Transform(hitBox.bbmin);
                var boneEndWorld = matrixBoneModelToWorld.Transform(hitBox.bbmax);
                var boneWorld = new Line3D(boneStartWorld, boneEndWorld);
                var (p0, p1) = aimRayWorld.ClosestPointsBetween(boneWorld, true);
                var distance = (p1 - p0).Length();
                if (distance < hitBox.radius * 0.9f /* trigger a little bit inside */)
                {
                    // intersects
                    return hitBoxId;
                }
            }

            return -1;
        }

        #endregion
    }
}
