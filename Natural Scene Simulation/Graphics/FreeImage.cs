using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{

    public enum FIF
    {
        FIF_UNKNOWN = -1,
        FIF_BMP = 0,
        FIF_ICO = 1,
        FIF_JPEG = 2,
        FIF_JNG = 3,
        FIF_KOALA = 4,
        FIF_LBM = 5,
        FIF_MNG = 6,
        FIF_PBM = 7,
        FIF_PBMRAW = 8,
        FIF_PCD = 9,
        FIF_PCX = 10,
        FIF_PGM = 11,
        FIF_PGMRAW = 12,
        FIF_PNG = 13,
        FIF_PPM = 14,
        FIF_PPMRAW = 15,
        FIF_RAS = 16,
        FIF_TARGA = 17,
        FIF_TIFF = 18,
        FIF_WBMP = 19,
        FIF_PSD = 20,
        FIF_CUT = 21,
        FIF_IFF = FIF_LBM,
        FIF_XBM = 22,
        FIF_XPM = 23
    }

    public class FreeImage
    {
        [DllImport("FreeImage.dll")]
        public static extern int FreeImage_Load(FIF format, string filename, int flags);

        [DllImport("FreeImage.dll")]
        public static extern void FreeImage_Unload(int handle);

        [DllImport("FreeImage.dll")]
        public static extern bool FreeImage_Save(FIF format, int handle, string filename, int flags);
    }
}
