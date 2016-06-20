using CH3.Utils;
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
        public BiDictionary<int, RoadNode> roadNodes { get; private set; }
        public BiDictionary<int, Road> roads { get; private set; }
        private IDController roadNodeIDs;
        private IDController roadIDs;

        public Map()
        {
            roadNodes = new BiDictionary<int, RoadNode>();
            roads = new BiDictionary<int, Road>();
            roadIDs = new IDController();
            roadNodeIDs = new IDController();
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

        public void AddRoadNode(Vector3 position)
        {
            int id = roadNodeIDs.Next();
            RoadNode roadNode = new RoadNode(id, position);
            roadNodes.Add(id, roadNode);
        }

        public void AddRoad(RoadNode fromNode, RoadNode toNode)
        {
            int id = roadIDs.Next();
            Road road = new Road(id, fromNode, toNode);
            roads.Add(id, road);
        }

        /// <summary>
        /// Deletes the road from the roadmap, but not from graphics!
        /// </summary>
        /// <param name="road">The road to delete</param>
        public void DeleteRoad(Road road)
        {
            road.fromNode.roads.Remove(road);
            road.toNode.roads.Remove(road);
            roads.Remove(road);
            roadIDs.Remove(road.id);
        }

        /// <summary>
        /// Deletes the roadNode and all roads connected to it, but not from graphics!
        /// </summary>
        /// <param name="roadNode">The roadNode to remove</param>
        public void DeleteRoadNode(RoadNode roadNode)
        {
            foreach (Road road in roadNode.roads)
            {
                DeleteRoad(road);
            }
            roadNodes.Remove(roadNode);
            roadNodeIDs.Remove(roadNode.id);
        }

        private string CreateXML()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(new StringWriter(sb), xws))
            {
                writer.WriteStartElement("roadmap");
                foreach (RoadNode node in roadNodes.GetValues<RoadNode>())
                {
                    writer.WriteStartElement("node");
                    writer.WriteAttributeString("id", node.id.ToString());
                    writer.WriteAttributeString("x", node.position.x.ToString());
                    writer.WriteAttributeString("y", node.position.y.ToString());
                    writer.WriteAttributeString("z", node.position.z.ToString());
                    writer.WriteEndElement();
                }
                foreach (Road road in roads.GetValues<Road>())
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
                int id = 0, iFromNode = 0, iToNode = 0;
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
                                roadNodeIDs.Add(id);
                                x = float.Parse(reader.GetAttribute("x"));
                                y = float.Parse(reader.GetAttribute("y"));
                                z = float.Parse(reader.GetAttribute("z"));
                                RoadNode node = new RoadNode(id, new Vector3(x, y, z));
                                this.roadNodes.Add(id, node);
                                break;
                            case "road":
                                id = int.Parse(reader.GetAttribute("id"));
                                roadIDs.Add(id);
                                iFromNode = int.Parse(reader.GetAttribute("fromnode"));
                                RoadNode fromNode = this.roadNodes.Get(iFromNode);
                                iToNode = int.Parse(reader.GetAttribute("tonode"));
                                RoadNode toNode = this.roadNodes.Get(iToNode);
                                Road road = new Road(id, fromNode, toNode);
                                fromNode.roads.Add(road);
                                toNode.roads.Add(road);
                                roads.Add(id, road);
                                break;
                        }
                    }
                }
            }
        }
    }
}