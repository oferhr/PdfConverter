//============================================================================
// File: Program.cs
// Purpose: Application entry point for PdfConverter
//
// Description:
//   This is the main entry point for the PdfConverter Windows Forms application.
//   It initializes the application, sets up licensing for PDF libraries
//   (IronPDF and Spire.PDF), and launches the main form (Form1).
//
// Key Responsibilities:
//   - Configure Windows Forms visual styles and DPI awareness
//   - Initialize IronPDF license for PDF conversion and merging
//   - Initialize Spire.PDF license as backup PDF library
//   - Launch the main application window (Form1)
//
// Dependencies:
//   - IronPdf (licensed) - Primary PDF processing library
//   - Spire.Pdf (licensed) - Secondary PDF processing library
//   - Windows Forms - UI framework
//
// Notes:
//   - License keys are embedded in code (consider moving to secure config)
//   - Application runs in STA (Single-Threaded Apartment) mode for COM interop
//   - High DPI mode is set to SystemAware for proper scaling
//============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfConverter
{
    /// <summary>
    /// Main program class containing the application entry point
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the PdfConverter application.
        /// Initializes visual styles, DPI settings, PDF library licenses, and launches the main form.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configure Windows Forms visual styles and DPI settings
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize IronPDF license (Primary PDF processing library)
            // License valid through: Feb 25, 2028
            IronPdf.License.LicenseKey = "IRONPDF.ERANMOR.IRO230226.9271.10135.622032-2DD0FD6081-KKPMPWATUQCCO-V2KQEWDNF34E-CLNVNYIYJKCB-AEDVOLCTLZHF-UGOPMILN7BQ7-J6UI42-LZC54LOUHIOTUA-LITE.5YR-UOUSZX.RENEW.SUPPORT.25.FEB.2028";

            // Initialize Spire.PDF license (Secondary PDF processing library)
            Spire.License.LicenseProvider.SetLicenseKey("WAeVMazkpJzcw3UBAJZ4Unw/c/R9zLOsl0B/0Hhr+CQp2FvoCpAY/acsTVSptMDXkARy8TtW+hZLUZukDAbIzQNhaxl44LnpgxrGjrsz4dRw37UtfF3OhEU3AzKoLMJH2Sa8YQ9eSKSNmfbC6grdVj/UhH3fR69Kuw5WfKoAFjRn4QaknkDy9CoUlu4Ut8GIxgJJrDh8pjv3lxJoQbMpyx6T61lLlyykZ+aSlXqKCbXMa+RuzSxQZgXrk3UqXSBuxonP4hKOxbIXTw5FKnhP1QuuyUKf2xs6VBh7pxtUlx7n7XIX9cWC/80HUHWfDlVgzzyMCB/qjUp0h9AqxtW3GgdXiMUsWpxvhsZiX/kQ7ggtFXo4RBakbdn1WtDedmzMXZuoFHuUB8GlDqpPwaZzUsbxkP+lrYv2WOFXsiBKU+gBxcyX99kno4eSd5OicoJuAnis9bzXCqNcpSJvS5/8Y82Mtpbw4vO7ZyYJgzh7tKbRRDb5uz4am0MJPsGDuwLu4g4z9XCjhdjtuVNiFFak66py2XiAtz6t1lgS/WsraATNE4osgVXhXQLOKEcRF5NlSBdSMSdvG16xzyMK9umwA7Kqldk/NCNcrTzm4dTo24vk/+0nah90T/OzM56gZWlFFo8lPR7X4/SvxF0nNvTtpOphjzfwzCY5TPOT9eIuKlEhwn9eIuKlEhwn6tUBDXQJwS7SWnZP+yZDBuSSXVd0w+I2hkYvOjtDHtwGh3xJuzzAOg4AoXWrPZtd4RUjBJWU3QfHd/PLDz5fBL9t3XjLQBA9OOlKFGxnJF+RN4FLUrbpjAJDA+sye/jVE7u5m7CPRJ1mCN7rx3llTOcfv8JPOwFk3pr/vksPF90ZbdSGJIoe7sA//cZTxzqv96Bn2mTxH+PFaGyUuwubm1qEOQIetj1QIWg79MYyLlLxaVe+Qj40OgFsKuklHJNqHwaoO15ky1ap17dnla6EO1AjWygGj0MxPncrc0FZWHIr6SDmXefrOlNicfhitFW+7zYKeUWPhX0ABfEDDPScs4T0rAyYQwvUrxO7qkdZbey1Dsmsdo3gP6e6IATSHFnC+B7r0ukVNujZ5kW1ocgGunSaRmK8cI4l/HoWW5CUBw+3ryS/WNoAZcZncTVt1WRofBH+jaMV1vtjtAzpi9Qy0XeEwOml6hn8Ddah/gG5k8rfJ3NYBwC9lB4kG1rK59s3oVpOhnIrtVGtF8YpHQ4oZqMYu86UiAK2gXoO5524thhN/2Aa4QgKvdEwnDHUYvSDC4Gbc8UQGbmixxfQ2NsLwgij8svpuszMyRzEfnsy+bUoNixvMA+28LhlKlAez689wdb+FZtieQhcSgdEa2t5siQhiueCjbIimLinEcmMbezq1awCFXawvgnjZt+eMpZohxl9egAdW1lHW2LhA000t2XUlmfyncHxEswxEminV8ncJZRnbF");

            // Legacy IronPDF license (expired Jan 19, 2022) - kept for reference
            //  IronPdf.License.LicenseKey = "IRONPDF.ERANMOR.IRO210119.6673.61137.911012-E4A858C1E3-BVFV36ZMBODS75L-ZO7KIF2CSE6J-HSFYXRNZBPLL-YDMZFGUBQ6CZ-BRTEE3GQAWOA-NPJWTO-LDJM7U3RQN2DUA-PRO.1DEV.1YR-BJNCXP.RENEW.SUPPORT.19.JAN.2022";

            // Launch the main application window
            Application.Run(new Form1());
        }
    }
}
