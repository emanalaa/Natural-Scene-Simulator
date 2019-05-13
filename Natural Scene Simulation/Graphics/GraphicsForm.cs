using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using GlmNet;
using System.Diagnostics;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
       
        Renderer renderer = new Renderer();
        Thread MainLoopThread;
        float deltaTime;
        Camera cam = new Camera();

        float xComp, zComp, yComp;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();

            MoveCursor();

            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
           
            MainLoopThread.Start();

        }
        void initialize()
        {
            renderer.Initialize();   
        }
        
        void MainLoop()
        {
            while (true)
            {
                renderer.Draw();
                renderer.Update(deltaTime);
                simpleOpenGlControl1.Refresh();
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();
            renderer.Update(deltaTime);
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = 0.3f;

            vec3 temp = renderer.camera.GetCameraPosition();

            if (e.KeyChar == 'a' )
                renderer.camera.Strafe(-speed);
            if (e.KeyChar == 'd')
                renderer.camera.Strafe(speed);
            if (e.KeyChar == 's')
                renderer.camera.Walk(-speed);
            if (e.KeyChar == 'w')
                renderer.camera.Walk(speed);
            if (e.KeyChar == 'z')
                renderer.camera.Fly(-speed);
            if (e.KeyChar == 'c' )
                renderer.camera.Fly(speed);

            /* if ((renderer.camera.mPosition.x > -50 && renderer.camera.mPosition.x < 50) && (renderer.camera.mPosition.y > -50 && renderer.camera.mPosition.y < 50) && (renderer.camera.mPosition.z > -50 && renderer.camera.mPosition.z < 50))
             {
                 temp = renderer.camera.GetCameraPosition();
             }
             else
             {
                 renderer.camera.mPosition = temp;
             }*/

            xComp = (renderer.camera.mPosition.x) + 30;
            zComp = (renderer.camera.mPosition.z) + 100;
            yComp = (renderer.heights[(int)xComp, (int)zComp]) - 65;
            renderer.camera.mPosition.y = yComp * 0.6f;

        }

        float prevX, prevY;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 0.1f;
            float delta = e.X - prevX;
            if (delta > 2)
                renderer.camera.Yaw(-speed);
            else if (delta < -2)
                renderer.camera.Yaw(speed);

            delta = e.Y - prevY;
            if (delta > 2)
                renderer.camera.Pitch(-speed);
            else if (delta < -2)
                renderer.camera.Pitch(speed);

            MoveCursor();
        }

        private void simpleOpenGlControl1_Load(object sender, System.EventArgs e)
        {

        }

        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X+simpleOpenGlControl1.Size.Width/2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }
    }
}
