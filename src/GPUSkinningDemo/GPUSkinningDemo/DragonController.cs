using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Input.Keyboard;
using WaveEngine.Components.Animation;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace GPUSkinningDemo
{
    public class DragonController : Component
    {
        [BindComponent]
        private Animation3D animation3D;

        [BindComponent]
        private Transform3D transform;

        [BindComponent]
        private HierarchyLineDrawable hiearchyLines;

        private KeyboardDispatcher keyboardDispacher;

        private string[] animations = new string[] { "flying", "running", "idle" };

        private int animIndex = 0;

        protected override bool OnAttached()
        {
            if (!Application.Current.IsEditor)
            {
                var display = this.Managers.RenderManager.ActiveCamera3D.Display;
                if (display != null)
                {
                    this.keyboardDispacher = display.KeyboardDispatcher;

                    if (this.keyboardDispacher != null)
                    {
                        this.keyboardDispacher.KeyUp += this.OnKeyUp;
                    }
                }

                this.animIndex = 2;
                this.CycleAnimation();
            }

            return base.OnAttached();
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            if (!Application.Current.IsEditor)
            {
                if (this.keyboardDispacher != null)
                {
                    this.keyboardDispacher.KeyUp -= this.OnKeyUp;
                }
            }
        }

        private void OnKeyUp(object sender, WaveEngine.Common.Input.Keyboard.KeyEventArgs e)
        {
            if (e.Key == Keys.Space)
            {
                this.CycleAnimation();
            }
            else if (e.Key == Keys.H)
            {
                this.hiearchyLines.IsEnabled = !this.hiearchyLines.IsEnabled;
            }
        }

        private void CycleAnimation()
        {
            this.animIndex = (this.animIndex + 1) % this.animations.Length;
            this.animation3D.CurrentAnimation = this.animations[this.animIndex];

            // We adjust the flying animation main position.
            this.transform.Position = (this.animIndex == 0) ? Vector3.Up * -4 : Vector3.Zero;
        }
    }
}
