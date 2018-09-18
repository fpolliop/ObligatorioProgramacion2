using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server1
{
    public partial class Board : Form
    {
        int TOTAL_TERRAIN_ROW_SPACES = 8;
        int TOTAL_TERRAIN_COLUMN_SPACES = 8;
        int TERRAIN_SPACE_SIZE = 50;
        int TERRAIN_SPACE_DISTANCE = 5;
        public Board()
        {
            InitializeComponent();
            groupBox_Terrain.Width = 445;
            groupBox_Terrain.Height = 450;

            PictureBox[,] terrainSpaces = new PictureBox[8, 8];
            int yLocation = 10;

            for (int i = 0; i < TOTAL_TERRAIN_ROW_SPACES; i++)
            {
                int xLocation = 5;
                for (int j = 0; j < TOTAL_TERRAIN_COLUMN_SPACES; j++)
                {
                    PictureBox newSpace = new PictureBox();
                    newSpace.BorderStyle = BorderStyle.FixedSingle;
                    newSpace.Name = "terrainSpace_" + i + "_" + j;
                    newSpace.Location = new Point(xLocation, yLocation);
                    newSpace.Width = TERRAIN_SPACE_SIZE;
                    newSpace.Height = TERRAIN_SPACE_SIZE;

                    xLocation += TERRAIN_SPACE_SIZE + TERRAIN_SPACE_DISTANCE;

                    groupBox_Terrain.Controls.Add(newSpace);
                }
                yLocation += TERRAIN_SPACE_SIZE + TERRAIN_SPACE_DISTANCE;
            }
        }

        private void button_StartGame_Click(object sender, EventArgs e)
        {
            Facade.StartGame();
            button_StartGame.Invalidate();
        }
    }
}
