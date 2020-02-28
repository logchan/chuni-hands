using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraHelperTest {
    class Program {
        static void Main(string[] args) {
            var cameras = CameraHelper.CameraHelper.GetCameras();
            foreach (var camera in cameras) {
                Console.WriteLine($"{camera.Id} {camera.Name}");
            }

            Console.ReadLine();
        }
    }
}
