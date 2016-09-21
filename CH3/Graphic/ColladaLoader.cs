using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collada141;

namespace CH3.Graphic
{
    class ColladaLoader
    {

        public void Load(string path)
        {
            COLLADA model = COLLADA.Load(path);
            // Iterate on libraries
            /*foreach (var item in model.Items)
            {
                if (item is library_animations)
                {
                    var anim = item as library_animations;
                    Console.WriteLine(anim.ToString());
                }
                var geometries = item as library_geometries;
                if (geometries == null)
                    continue;

                // Iterate on geomerty in library_geometries 
                foreach (var geom in geometries.geometry)
                {
                    var mesh = geom.Item as mesh;
                    if (mesh == null)
                        continue;

                    // Dump source[] for geom
                    foreach (var source in mesh.source)
                    {
                        var float_array = source.Item as float_array;
                        if (float_array == null)
                            continue;

                        Console.WriteLine("Geometry {0} source {1} : ", geom.id, source.id);
                        foreach (var mesh_source_value in float_array.Values)
                            Console.WriteLine("{0} ", mesh_source_value);
                        Console.WriteLine();
                    }

                    // Dump Items[] for geom
                    foreach (var meshItem in mesh.Items)
                    {

                        if (meshItem is vertices)
                        {
                            var vertices = meshItem as vertices;
                            var inputs = vertices.input;
                            foreach (var input in inputs)
                                Console.WriteLine("\t Semantic {0} Source {1}", input.semantic, input.source);
                        }
                        else if (meshItem is triangles)
                        {
                            var triangles = meshItem as triangles;
                            var inputs = triangles.input;
                            foreach (var input in inputs)
                                Console.WriteLine("\t Semantic {0} Source {1} Offset {2}", input.semantic, input.source, input.offset);
                            Console.WriteLine("\t Indices {0}", triangles.p);
                        }
                    }
                }
            }*/
        }
    }
}
