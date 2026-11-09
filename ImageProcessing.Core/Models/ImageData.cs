namespace Core.Models;

public class ImageData
{
    public int Height { get; }
    public int Width { get; }
    
    public byte[] Pixels { get; }
    
    public ImageData(int height, int width, byte[] pixels)
    {
        if (height <= 0)
        {
            throw new ArgumentException("The image height must be greater than 0.");
        }
        
        if (width <= 0)
        {
            throw new ArgumentException("The image width must be greater than 0.");
        }
        
        if (pixels == null)
        {
            throw new ArgumentNullException(nameof(pixels));
        }

        if (height * width > pixels.Length)
        {
            throw new ArgumentException("The image dimensions do not correspond to the number of pixels.");
        }
        
        Height = height;
        Width = width;
        Pixels = pixels;
    }

    public byte GetPixel(int x, int y)
    {
        if (x >= Width || x < 0)
        {
            throw new ArgumentException($"The value of x must be in the range from 0 to {Width - 1}.");
        }
        
        if (y >= Height ||  y < 0)
        {
            throw new ArgumentException($"The value of y must be in the range from 0 to {Height - 1}.");
        }

        return Pixels[Width * y + x];
    }
}