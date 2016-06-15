using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CH3.Map
{
    public class Map
    {
        public Dictionary<int, RoadNode> roadNodes { get; private set; }
        public Dictionary<int, Road> roads { get; private set; }

        public Map()
        {
            roadNodes = new Dictionary<int, RoadNode>();
            roads = new Dictionary<int, Road>();
        }

        public void LoadMap(string path)
        {
            string content = ReadMap(path);
            ParseMap(content);
        }

        public void WriteMap(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(CreateXML());
            }
        }

        private string CreateXML()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(new StringWriter(sb), xws))
            {
                writer.WriteStartElement("roadmap");
                foreach (RoadNode node in roadNodes.Values)
                {
                    writer.WriteStartElement("node");
                    writer.WriteAttributeString("id", node.id.ToString());
                    writer.WriteAttributeString("x", node.position.x.ToString());
                    writer.WriteAttributeString("y", node.position.y.ToString());
                    writer.WriteAttributeString("z", node.position.z.ToString());
                    writer.WriteEndElement();
                }
                foreach (Road road in roads.Values)
                {
                    writer.WriteStartElement("road");
                    writer.WriteAttributeString("id", road.id.ToString());
                    writer.WriteAttributeString("fromnode", road.fromNode.id.ToString());
                    writer.WriteAttributeString("tonode", road.toNode.id.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            return sb.ToString();
        }

        private string ReadMap(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        private void ParseMap(string data)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(data)))
            {
                int id = 0, fromnode = 0, tonode = 0;
                float x = 0, y = 0, z = 0;
                reader.ReadToFollowing("roadmap");
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name.ToString())
                        {
                            case "node":
                                id = int.Parse(reader.GetAttribute("id"));
                                x = float.Parse(reader.GetAttribute("x"));
                                y = float.Parse(reader.GetAttribute("y"));
                                z = float.Parse(reader.GetAttribute("z"));
                                RoadNode node = new RoadNode(id, new Vector3(x, y, z));
                                roadNodes.Add(id, node);
                                break;
                            case "road":
                                id = int.Parse(reader.GetAttribute("id"));
                                fromnode = int.Parse(reader.GetAttribute("fromnode"));
                                tonode = int.Parse(reader.GetAttribute("tonode"));
                                Road road = new Road(id, roadNodes[fromnode], roadNodes[tonode]);
                                roads.Add(id, road);
                                break;
                        }
                    }
                }
            }
        }
    }
}