namespace RdClient.Shared.Models.Viewport
{
    public class Panel : IViewportPanel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        private IViewportTransform _transform;
        public IViewportTransform Transform { get { return _transform; } }       

        public Panel()
        {
            _transform = new TransformX(this);
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }
    }
}
