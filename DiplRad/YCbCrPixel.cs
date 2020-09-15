using System.Drawing;

namespace DiplRad
{
    class YCbCrPixel
    {
        internal double _y;
        internal double _Cb;
        internal double _Cr;

        public YCbCrPixel(Color color)
        {
            _y = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B - 128;
            _Cb = -0.1687 * color.R - 0.3313 * color.G + 0.500 * color.B;
            _Cr = 0.500 * color.R - 0.4187 * color.G - 0.0813 * color.B;
        }

        public double GetComponent(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.Y:
                    return _y;
                case ComponentType.Cb:
                    return _Cb;
                default:
                    return _Cr;
            };
        }

        public enum ComponentType
        {
            Y = 0,
            Cb = 1,
            Cr = 2,
        }
    }
}
