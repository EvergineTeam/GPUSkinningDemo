// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using WaveEngine.Common.Graphics;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace GPUSkinningDemo
{
    public class HierarchyLineDrawable : Drawable3D
    {
        [BindService]
        GraphicsContext graphicsContext;

        private RenderLayerDescription renderLayerDescription;
        private LineBatch3D lineBatch3D;

        public RenderLayerDescription RenderLayerDescription
        {
            get => this.renderLayerDescription;

            set
            {
                this.renderLayerDescription = value;

                if (this.IsAttached)
                {
                    this.RefreshResources();
                }
            }
        }

        public float AxisSize = 0.1f;

        private void ClearResources()
        {
            if (this.lineBatch3D != null)
            {
                this.lineBatch3D.Dispose();
                this.lineBatch3D = null;
            }
        }

        private void RefreshResources()
        {
            this.ClearResources();

            if (this.renderLayerDescription != null)
            {
                this.lineBatch3D = new LineBatch3D(graphicsContext, this.renderLayerDescription);
                this.RenderManager.AddRenderObject(this.lineBatch3D);
            }
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            if (this.lineBatch3D != null)
            {
                this.Managers.RenderManager.RemoveRenderObject(this.lineBatch3D);
            }
        }

        /// <inheritdoc/>
        protected override bool OnAttached()
        {
            var result = base.OnAttached();
            this.RefreshResources();

            return result;
        }

        /// <inheritdoc/>
        public override void Draw(DrawContext drawContext)
        {
            this.DrawEntityPosition(this.Owner);
        }

        private void DrawEntityPosition(Entity entity)
        {
            var transform = entity.FindComponent<Transform3D>();

            Vector3 myPosition = transform.Position;

            Quaternion myOrientation = transform.Orientation;
            Vector3 myScale = Vector3.One;
            Matrix4x4.CreateFromTRS(ref myPosition, ref myOrientation, ref myScale, out var transformAxis);

            this.lineBatch3D.DrawAxis(transformAxis, this.AxisSize);

            if (entity.Parent != null)
            {
                Vector3 parentPosition = entity.Parent.FindComponent<Transform3D>().Position;
                this.lineBatch3D.DrawLine(myPosition, parentPosition, Color.Lime);
            }

            foreach (var c in entity.ChildEntities)
            {
                this.DrawEntityPosition(c);
            }
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.ClearResources();
        }
    }
}
