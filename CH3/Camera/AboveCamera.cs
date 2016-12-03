using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Keyboard;

namespace CH3.Camera
{
    public class AboveCamera : ThirdPersonCamera
    {
        public Vector4 visible { get; private set; }
        public float height { get; set; }
        public bool fixedRotation { get; set; }

        public Vector2 halfSize { get; private set; }

        public AboveCamera() : base()
        {
            //Input.SubscribeKeyPressed(KeyDown);
            fixedRotation = true;

            y_fov = CalcFovY(60.0f, 16.0f, 9.0f);
            x_fov = CalcFovX(y_fov, (float)Window.WIDTH, (float)Window.HEIGHT);
            projection = Matrix4.CreatePerspectiveFieldOfView(y_fov * (float)(Math.PI / 180.0),
                ((float)Window.WIDTH / (float)Window.HEIGHT), (float)z_near, (float)z_far);

            projection_opentk = OpenTK.Matrix4.CreatePerspectiveFieldOfView(y_fov * (float)(Math.PI / 180.0),
                ((float)Window.WIDTH / (float)Window.HEIGHT), (float)z_near, (float)z_far);

            halfSize = new Vector2(
                (float)Math.Tan((x_fov * (Math.PI / 180.0f)) * 0.5f),
                (float)Math.Tan((y_fov * (Math.PI / 180.0f)) * 0.5f));
        }
        /*
        private void KeyDown(Key key)
        {
            switch (key)
            {
                case Key.Space:
                    height += 10;
                    break;
                case Key.C:
                    height -= 10;
                    break;
                case Key.O:
                    print_debug();
                    break;
            }
        }*/

        public override void Update(Vector2 mouse)
        {
            if (CH3.Input.IsKeyActive(Key.PageUp)) height += 5;
            if (CH3.Input.IsKeyActive(Key.PageDown)) height -= 5;

            if(follow.has_physics) {
                OpenTK.Vector3 _pos = follow.body.WorldTransform.ExtractTranslation();
                target = new Vector3(_pos.X, _pos.Y, _pos.Z);
                position = new Vector3(_pos.X, _pos.Y, height); 
            } else { 
                position = new Vector3(follow.position.x, follow.position.y, height);
                target = follow.position;
            }
            
            up = Vector3.Up;
            if (!fixedRotation)
            {
                up *= Matrix4.CreateRotationZ(-follow.rotationZ);
            }

            Vector2 diff = halfSize * height;

            visible = new Vector4(
                -diff.x + follow.position.x,
                 diff.x + follow.position.x,
                -diff.y + follow.position.y,
                 diff.y + follow.position.y);

            UpdateMatrices();
        }
    }
}
