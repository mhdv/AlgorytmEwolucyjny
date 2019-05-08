using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using static ILNumerics.Globals;
using static ILNumerics.ILMath;

namespace AlgorytmEwolucyjny
{
    public partial class PlotSurface : Form
    {
        PlotCube scene = new PlotCube();

        public PlotSurface(PlotCube sc)
        {
            scene = sc;
            InitializeComponent();
        }

        // Initial plot setup, modify this as needed
        private void ilPanel1_Load(object sender, EventArgs e)
        {
            ilPanel1.Scene.Add(scene);
        }

        /// <summary>
        /// Example update function used for dynamic updates to the plot
        /// </summary>
        /// <param name="A">New data, matching the requirements of your plot</param>
        public void Update(InArray<double> A)
        {
            using (Scope.Enter(A))
            {

                // fetch a reference to the plot
                var plot = ilPanel1.Scene.First<LinePlot>(tag: "mylineplot");
                if (plot != null)
                {
                    // make sure, to convert elements to float
                    plot.Update(ILMath.tosingle(A));
                    //
                    // ... do more manipulations here ...

                    // finished with updates? -> Call Configure() on the changes 
                    plot.Configure();

                    // cause immediate redraw of the scene
                    ilPanel1.Refresh();
                }

            }
        }

        /// <summary>
        /// Example computing module as private class 
        /// </summary>
        private class Computation
        {
            /// <summary>
            /// Create some test data for plotting
            /// </summary>
            /// <param name="ang">end angle for a spiral</param>
            /// <param name="resolution">number of points to plot</param>
            /// <returns>3d data matrix for plotting, points in columns</returns>
            public static RetArray<float> CreateData(int ang, int resolution)
            {
                using (Scope.Enter())
                {
                    Array<float> A = linspace<float>(0, ang * pif, resolution);
                    Array<float> Pos = zeros<float>(3, A.S[1]);
                    Pos[0, full] = sin(A);
                    Pos[1, full] = cos(A);
                    Pos[2, full] = A;
                    return Pos;
                }
            }

        }
    }
}
