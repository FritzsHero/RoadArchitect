namespace RoadArchitect
{
    // RenderQueue provides ID's for Unity render queues. These can be applied to sub-shader tags,
    // but it's easier to just set material.renderQueue. Static class instead of enum because these
    // are int's, so this way client code doesn't need to use typecasting.
    //
    // From the documentation:
    // For special uses in-between queues can be used. Internally each queue is represented
    // by integer index; Background is 1000, Geometry is 2000, Transparent is 3000 and
    // Overlay is 4000.
    //
    // NOTE: Keep these in numerical order for ease of understanding. Use plurals for start of
    // a group of layers.
    public static class RenderQueue
    {
        public const int Background = 1000;

        // Mid-ground.
        // +1, 2, 3, ... for additional layers
        public const int ParallaxLayers = Background + 100;

        // Lines on the ground.
        public const int GroundLines = Background + 200;

        public const int Tracks = GroundLines + 0;
        public const int Routes = GroundLines + 1;
        public const int IndicatorRings = GroundLines + 2;
        public const int Road = GroundLines + 3;

        public const int Geometry = 2000;


        public const int Transparent = 3000;

        // Lines on the screen. (Over world, but under GUI.)
        public const int ScreenLines = Transparent + 100;

        public const int Overlay = 4000;
    }
}
