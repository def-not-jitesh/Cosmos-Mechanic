/* there are two types of tilesets, the objects and normal tilesets
 * normal tilesets have one image source 
 * but the object tilsets are a collection of images of different sizes */	

/* the cell will be rendered according to which tileset it belongs
 * the normal tileset has a image child, which the object tileset does not */
					
using System; 
using System.Collections.Generic;
using System.Xml; 
using System.Text.RegularExpressions; 
using System.Numerics; 
using System.Linq; 
using System.IO; 
using Raylib_cs; 
using static Raylib_cs.Raylib; 

using Game.Utility; 

namespace Game.Render; 

public class Map {
	
	struct Tileset {
		public int rows; 
		public int columns; 
		public int tileheight; 
		public int tilewidth; 
		public XmlNode root; 
		public Texture2D texture; // for normal tilesets 
		public List<Texture2D> objectImageTextures; // for object tilesets
		public List<(int imageWidth, int imageHeight)> objectImageSizes; // size for every image in object tilesets	
	}; 

	Dictionary<string, Tileset> sourceTilesetMap = new Dictionary<string, Tileset>();
      	Dictionary<int, string> curGIDSourceMap = new Dictionary<int, string>(); 	
	List<int> curTilesetGIDs = new List<int>(); 
	XmlNode curRootNode; 

	public Map() {}

	public void init() {
	 	for (int sourceIndex = 0; sourceIndex < Utils.tilesetSourceFiles.Count; sourceIndex++) {
			XmlDocument curDoc = new XmlDocument(); 
			try { curDoc.Load(Path.Combine(Utils.basePath, "Assets", "Maps", Utils.tilesetSourceFiles[sourceIndex])); }
			catch (System.IO.FileNotFoundException) {
				Console.WriteLine(Utils.tilesetSourceFiles[sourceIndex] + ": file not found"); 
			}
			
			Tileset curTileset = new Tileset(); 
			XmlNode curRoot = curDoc.DocumentElement; 
			curTileset.root = curRoot; 
			if (curRoot.ChildNodes[0].Name != "image") {
				curTileset.objectImageTextures = new List<Texture2D>(); 
				curTileset.objectImageSizes = new List<(int imageWidth, int imageHeight)>(); 

				// starting from 1 because first child is "grid" in object tilesets 
				for (int childIndex = 1; childIndex < curRoot.ChildNodes.Count; childIndex++) {
					string tilesetSource = curRoot.ChildNodes[childIndex].ChildNodes[0].Attributes[0].Value;
					string trimmedSource = Regex.Replace(tilesetSource, @"^\.\./", ""); 
					string[] object_parts = trimmedSource.Split('/');
					object_parts = object_parts.Prepend("Assets").ToArray(); 
					object_parts = object_parts.Prepend(Utils.basePath).ToArray(); 

					curTileset.objectImageTextures.Add(LoadTexture(Path.Combine(object_parts))); 
					curTileset.objectImageSizes.Add((int.Parse(curRoot.ChildNodes[childIndex].ChildNodes[0].Attributes[1].Value), 
							int.Parse(curRoot.ChildNodes[childIndex].ChildNodes[0].Attributes[2].Value))); 
				}
				
			} else {
				
				string tilesetSource = curRoot.ChildNodes[0].Attributes[0].Value; 
				string trimmedSource = Regex.Replace(tilesetSource, @"^\.\./", ""); 
				string[] tile_parts = trimmedSource.Split('/');
			       	tile_parts = tile_parts.Prepend("Assets").ToArray(); 
				tile_parts = tile_parts.Prepend(Utils.basePath).ToArray();

				curTileset.texture = LoadTexture(Path.Combine(tile_parts)); 
				for (int attrIndex = 0; attrIndex < curRoot.Attributes.Count; attrIndex++) {
					switch (curRoot.Attributes[attrIndex].Name) {
						case "tilewidth":
							curTileset.tilewidth = int.Parse(curRoot.Attributes[attrIndex].Value);
							break; 
						case "tileheight":
							curTileset.tileheight = int.Parse(curRoot.Attributes[attrIndex].Value);
							break;
						case "tilecount":
							curTileset.rows = int.Parse(curRoot.Attributes[attrIndex].Value);
							break; 
						case "columns":
							curTileset.columns = int.Parse(curRoot.Attributes[attrIndex].Value);
							break; 
						
					}
				}
			}

			sourceTilesetMap.Add(Utils.tilesetSourceFiles[sourceIndex], curTileset); 
		}
	}

	public void LoadMap(Constants.Room map) {
		string path = Path.Combine(Utils.basePath, "Assets", "Maps", Utils.roomMapSourceFile[map]); 
		XmlDocument doc = new XmlDocument(); 
		try { doc.Load(path); }
		catch (System.IO.FileNotFoundException) {
			Console.WriteLine(Utils.roomMapSourceFile[map] + ": file not found"); 
		}
		
		curRootNode = doc.DocumentElement; 

		for (int i = 0; i < curRootNode.ChildNodes.Count; i++) {
			if (curRootNode.ChildNodes[i].Name == "tileset") {
				string source = curRootNode.ChildNodes[i].Attributes[1].Value;
				curGIDSourceMap.Add(int.Parse(curRootNode.ChildNodes[i].Attributes[0].Value), source); 
				curTilesetGIDs.Add(int.Parse(curRootNode.ChildNodes[i].Attributes[0].Value)); 
			}
		}

	}
	
	public void update() {
		for (int i = 0; i < curRootNode.ChildNodes.Count; i++) {
			if (curRootNode.ChildNodes[i].Name == "layer") {
				// layer will have only one child node
				string layer = curRootNode.ChildNodes[i].ChildNodes[0].InnerText; 
				string cleanLayer = Regex.Replace(layer, @"\s+", ""); 

				int layerIndex = 0; 
				int cellCount = 0; 
				while (layerIndex < cleanLayer.Length) {
					string curCell = ""; 
					while (layerIndex < cleanLayer.Length && char.IsDigit(cleanLayer[layerIndex])) {
						curCell += cleanLayer[layerIndex];
						layerIndex++; 
					}
					
					int curCellNumber = int.Parse(curCell); 
					int curCellGID = -1;  
					for (int tsIndex = curTilesetGIDs.Count-1; tsIndex >= 0; tsIndex--) {
						if (curCellNumber >= curTilesetGIDs[tsIndex]) { 
							curCellGID = curTilesetGIDs[tsIndex];
							break; 
						}
					}

					if (curCellGID != -1) {
						int cell = curCellNumber - curCellGID; 
						int colScreen = cellCount % Constants.tileCountRow; 
						int rowScreen = cellCount / Constants.tileCountRow; 
						if (sourceTilesetMap[curGIDSourceMap[curCellGID]].root.ChildNodes[0].Name == "image") {
							int colTileset = cell % sourceTilesetMap[curGIDSourceMap[curCellGID]].columns; 
							int rowTileset = cell / sourceTilesetMap[curGIDSourceMap[curCellGID]].columns; 
							
							DrawTexturePro(sourceTilesetMap[curGIDSourceMap[curCellGID]].texture,
									new Rectangle(colTileset * sourceTilesetMap[curGIDSourceMap[curCellGID]].tilewidth, 
										rowTileset * sourceTilesetMap[curGIDSourceMap[curCellGID]].tileheight,
									       	sourceTilesetMap[curGIDSourceMap[curCellGID]].tilewidth, 
										sourceTilesetMap[curGIDSourceMap[curCellGID]].tileheight),
									new Rectangle(colScreen * Utils.screenWidth/Constants.tileCountRow, 
										rowScreen * Utils.screenHeight/Constants.tileCountColumn, 
										Utils.screenWidth/Constants.tileCountRow, 
										Utils.screenHeight/Constants.tileCountColumn), 
									new Vector2(0, 0), 
									0f, 
									Color.White);
						} else {
							
							float destX = colScreen * Utils.screenWidth/Constants.tileCountRow; 
							float destY = (rowScreen * Utils.screenHeight/Constants.tileCountColumn) + Utils.screenHeight/Constants.tileCountColumn - sourceTilesetMap[curGIDSourceMap[curCellGID]].objectImageSizes[cell].imageHeight; 

							DrawTexturePro(sourceTilesetMap[curGIDSourceMap[curCellGID]].objectImageTextures[cell], 
								new Rectangle(0f, 0f,
								       	sourceTilesetMap[curGIDSourceMap[curCellGID]].objectImageSizes[cell].imageWidth, 
									sourceTilesetMap[curGIDSourceMap[curCellGID]].objectImageSizes[cell].imageHeight), 
								new Rectangle(destX, destY, 
									sourceTilesetMap[curGIDSourceMap[curCellGID]].objectImageSizes[cell].imageWidth, 
									sourceTilesetMap[curGIDSourceMap[curCellGID]].objectImageSizes[cell].imageHeight), 
								new Vector2(0, 0), 
								0f, 
								Color.White);  
						}
					}

					cellCount++; 
					layerIndex++; 
				}
			}
		} 
	}
}; 

