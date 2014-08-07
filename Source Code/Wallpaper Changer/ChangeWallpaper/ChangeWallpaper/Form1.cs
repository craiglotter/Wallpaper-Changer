using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO; 
using System.Data;
using Microsoft.Win32 ;

namespace SetWallpaper
{
	using System.Net ;

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
    {

        private string FileName_OUT = "";
        private string Style_OUT = "";
        private Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public Form1(string FileName_IN, string Style_IN)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            FileName_OUT = FileName_IN;
            Style_OUT = Style_IN;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Setting Wallpaper...";
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(197, 32);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Set Wallpaper";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        static void Main(string[] args) 
		{
            if (args.Length == 2)
			Application.Run(new Form1(args[0],args[1]));
                            
		}



        private void Form1_Load(object sender, EventArgs e)
        {
            try 
	        {
                Wallpaper.Style s2 = (Wallpaper.Style)Enum.Parse(typeof(Wallpaper.Style), Style_OUT, false);
                Wallpaper.Set(new Uri(FileName_OUT),s2);
                Close();
	        }   
	        catch (Exception)
        	{   
				throw;
        	}
        }
   	}

	public sealed class Wallpaper
	{
		Wallpaper( ) { }

		const int SPI_SETDESKWALLPAPER = 20  ;
		const int SPIF_UPDATEINIFILE = 0x01;
		const int SPIF_SENDWININICHANGE = 0x02;

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		static  extern int SystemParametersInfo (int uAction , int uParam , string lpvParam , int fuWinIni) ;

		public enum Style : int
		{
			Tiled,
			Centered,
			Stretched
		}

		public static void Set ( Uri uri, Style style )
		{
			System.IO.Stream s = new WebClient( ).OpenRead( uri.ToString( ) );

			System.Drawing.Image img = System.Drawing.Image.FromStream( s );
			string tempPath = Path.Combine( Path.GetTempPath( ), "wallpaper.bmp"  ) ;
			img.Save( tempPath ,  System.Drawing.Imaging.ImageFormat.Bmp ) ;

			RegistryKey key = Registry.CurrentUser.OpenSubKey( @"Control Panel\Desktop", true ) ;
			if ( style == Style.Stretched )
			{
				key.SetValue(@"WallpaperStyle", 2.ToString( ) ) ;
				key.SetValue(@"TileWallpaper", 0.ToString( ) ) ;
			}

			if ( style == Style.Centered )
			{
				key.SetValue(@"WallpaperStyle", 1.ToString( ) ) ;
				key.SetValue(@"TileWallpaper", 0.ToString( ) ) ;
			}

			if ( style == Style.Tiled )
			{
				key.SetValue(@"WallpaperStyle", 1.ToString( ) ) ;
				key.SetValue(@"TileWallpaper", 1.ToString( ) ) ;
			}

			SystemParametersInfo( SPI_SETDESKWALLPAPER, 
				0, 
				tempPath,  
				SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE );
		}
	}
}
