namespace Server1
{
    partial class Board
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox_Terrain = new System.Windows.Forms.GroupBox();
            this.button_StartGame = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // groupBox_Terrain
            // 
            this.groupBox_Terrain.Location = new System.Drawing.Point(150, 55);
            this.groupBox_Terrain.Name = "groupBox_Terrain";
            this.groupBox_Terrain.Size = new System.Drawing.Size(837, 688);
            this.groupBox_Terrain.TabIndex = 0;
            this.groupBox_Terrain.TabStop = false;
            // 
            // button_StartGame
            // 
            this.button_StartGame.Location = new System.Drawing.Point(957, 840);
            this.button_StartGame.Name = "button_StartGame";
            this.button_StartGame.Size = new System.Drawing.Size(165, 35);
            this.button_StartGame.TabIndex = 1;
            this.button_StartGame.Text = "Start Game";
            this.button_StartGame.UseVisualStyleBackColor = true;
            this.button_StartGame.Click += new System.EventHandler(this.button_StartGame_Click);
            // 
            // Board
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 912);
            this.Controls.Add(this.button_StartGame);
            this.Controls.Add(this.groupBox_Terrain);
            this.Name = "Board";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox_Terrain;
        private System.Windows.Forms.Button button_StartGame;
    }
}

