namespace ReportGenerator.Models
{
    public class RectangleD    {

        public double Left { get; set; }
        public double Bottom{ get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Right
        {
            get
            {
                return Left + Width;                
            }
        }
        public double Top
        {
            get
            {
                return Bottom + Height;
            }
        } 
        public RectangleD(double left, double bottom, double width, double height)
        {
            this.Left = left;
            this.Bottom = bottom;
            this.Width = width;
            this.Height = height;
        }
    }
}