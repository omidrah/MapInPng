using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportGenerator.Models;

namespace ReportGenerator.Controllers {
    [ApiController]
    [Route ("[controller]")]
    public class WeatherForecastController : ControllerBase {
        private static readonly string[] Summaries = new [] {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController (ILogger<WeatherForecastController> logger) {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get () {
            var rng = new Random ();
            return Enumerable.Range (1, 5).Select (index => new WeatherForecast {
                    Date = DateTime.Now.AddDays (index),
                        TemperatureC = rng.Next (-20, 55),
                        Summary = Summaries[rng.Next (Summaries.Length)]
                })
                .ToArray ();
        }

        [HttpGet]
        [Route ("[Action]")]
        public async Task GetPng (int zoom, double lat, double lon) {
            var fileInfo = new FileInfo ($"{Guid.NewGuid().ToString("N")}.png");
            var fs = System.IO.File.Create (fileInfo.FullName);
            var x = long2tilex (lon, zoom);
            var y = lat2tiley (lat, zoom);
            using (var client = new HttpClient ()) {
                //HTTP GET
                client.DefaultRequestHeaders.UserAgent.ParseAdd ("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                //var response = await client.GetAsync (new Uri ($"http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png"));                
                //response.EnsureSuccessStatusCode ();                
                //await using var ms = await response.Content.ReadAsStreamAsync ();
                //ms.Seek (0, SeekOrigin.Begin);
                //ms.CopyTo (fs);

                using var stream = await client.GetStreamAsync (new Uri ($"http://a.tile.openstreetmap.org/{zoom}/{x}/{y}.png"));
                //System.IO.File.OpenWrite(Guid.NewGuid().ToString("N")+".png")                
                stream.CopyTo (fs);
                //}
                Bitmap img = new Bitmap (System.Drawing.Image.FromStream (fs));
                Bitmap tempBitmap = new Bitmap (img.Width, img.Height);
                Graphics gr = Graphics.FromImage (tempBitmap);
                    //Graphics gr   Graphics.FromImage(img);
             
         //(gr);    //  Drarush(Color.
         //DrawString(gr);
                         //DrawCirecle(gr);

           gr.FillRectangle (new SolidBrush (Color.Yellow), 0, 0, 100, 100);         
                tempBitmap.Save ("abc0" + Guid.NewGuid ().ToString ("N") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        Graphics DrawLine (Graphics gr) {
            var c = new PointF (0, 0);
            var b = new PointF (100, 200);

            gr.DrawLine (new Pen (Brushes.DeepSkyBlue), c, b);
            return gr;
        }
        Graphics DrawRectangle (Graphics gr) {
            gr.FillRectangle (new SolidBrush (Color.White), 0, 0, 100, 100);
            return gr;
        }
        Graphics DrawString (Graphics gr) {
                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                gr.DrawString ("Omid Rahimi", drawFont,drawBrush, new RectangleF (0, 0, 100, 100));
            return gr;
        }
        Graphics DrawCirecle(Graphics gr)
        {
              Bitmap circle = new Bitmap(15, 15, PixelFormat.Format24bppRgb);                
                    gr.FillRectangle(Brushes.White, 0, 0, circle.Width, circle.Height);
                    SolidBrush brush = new SolidBrush(Color.Red);
                    gr.FillEllipse(brush, 0, 0, circle.Width, circle.Height);            
            return gr;
        }
        int long2tilex (double lon, int z) {
            return (int) (Math.Floor ((lon + 180.0) / 360.0 * (1 << z)));
        }
        int lat2tiley (double lat, int z) {
            return (int) Math.Floor ((1 - Math.Log (Math.Tan (ToRadians (lat)) + 1 / Math.Cos (ToRadians (lat))) / Math.PI) / 2 * (1 << z));
        }
        private double ToRadians (double val) {
            return (Math.PI / 180) * val;
        }
    }

}