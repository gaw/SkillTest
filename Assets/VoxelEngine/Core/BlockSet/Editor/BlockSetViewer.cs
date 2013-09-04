using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class BlockSetViewer {
	
	private static string DRAG_AND_DROP = "drag block";
	
	public static int SelectionGrid(BlockSet blockSet, int index, params GUILayoutOption[] options) {
		Container<Vector2> scroll = EditorGUIUtils.GetStateObject<Container<Vector2>>(blockSet.GetHashCode());
		
		scroll.value = GUILayout.BeginScrollView(scroll, options);
		index = SelectionGrid(blockSet.GetBlocks(), index);
		GUILayout.EndScrollView();
		
		return index;
	}
	
	private static int SelectionGrid(IList<Block> items, int index) {
		Rect rect;
		int xCount, yCount;
		index = SelectionGrid(items, index, out rect, out xCount, out yCount);
		float itemWidth = rect.width/xCount;
		float itemHeight = rect.height/yCount;
		
		GUI.BeginGroup(rect);
		Vector2 mouse = Event.current.mousePosition;
		int posX = Mathf.FloorToInt(mouse.x/itemWidth);
		int posY = Mathf.FloorToInt(mouse.y/itemHeight);
		int realIndex = -1; // номер элемента под курсором
		if(posX >= 0 && posX < xCount && posY >= 0 && posY < yCount) realIndex = posY*xCount + posX;
		
		int dropX = Mathf.Clamp(posX, 0, xCount-1);
		int dropY = Mathf.Clamp(posY, 0, yCount-1);
		if(dropY == yCount-1 && items.Count%xCount != 0) dropX = Mathf.Clamp(dropX, 0, items.Count%xCount);
		int dropIndex = dropY*xCount + dropX; // ближайший элемент к курсору
		
		if(Event.current.type == EventType.MouseDrag && Event.current.button == 0 && realIndex == index) {
			DragAndDrop.PrepareStartDrag();
			DragAndDrop.objectReferences = new Object[0];
			DragAndDrop.paths = new string[0];
			DragAndDrop.SetGenericData(DRAG_AND_DROP, new Container<int>(index));
			DragAndDrop.StartDrag("DragAndDrop");
			Event.current.Use();
		}
		
		if(Event.current.type == EventType.DragUpdated) {
			Container<int> data = (Container<int>)DragAndDrop.GetGenericData(DRAG_AND_DROP);
			if(data != null) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				Event.current.Use();
			}
		}
		
		if(Event.current.type == EventType.DragPerform) {
			Container<int> oldIndex = (Container<int>)DragAndDrop.GetGenericData(DRAG_AND_DROP);
			
			if(dropIndex > oldIndex.value) dropIndex--;
			dropIndex = Mathf.Clamp(dropIndex, 0, items.Count-1);
			Insert(items, dropIndex, oldIndex);
			
			index = dropIndex;
			
			DragAndDrop.AcceptDrag();
			DragAndDrop.PrepareStartDrag();
			Event.current.Use();
		}
		
		if(Event.current.type == EventType.Repaint && DragAndDrop.visualMode == DragAndDropVisualMode.Link) {
			Vector2 pos = new Vector2(2+dropX*itemWidth, 2+dropY*itemHeight);
			Rect lineRect = new Rect(pos.x-2, pos.y, 2, itemWidth-2);
			EditorGUIUtils.FillRect(lineRect, Color.red);
		}
		GUI.EndGroup();
		
		return index;
	}
	
	private static int SelectionGrid(IList<Block> items, int index, out Rect rect, out int xCount, out int yCount) {
		xCount = Mathf.FloorToInt( Screen.width/66f );
		yCount = Mathf.CeilToInt( (float) items.Count/xCount );
		
		rect = GUILayoutUtility.GetAspectRect((float)xCount/yCount);
		float labelHeight = GUI.skin.label.CalcHeight(GUIContent.none, 0); // высота текста
		GUILayout.Space(labelHeight*yCount);
		rect.height += labelHeight*yCount;
		
		Rect[] rects = GUIUtils.Separate(rect, xCount, yCount);
		for(int i=0; i<items.Count; i++) {
			Rect position = rects[i];
			position.xMin += 2;
			position.yMin += 2;
				
			bool selected = DrawItem(position, items[i], i == index, i);
			if(selected) index = i;
		}
		
		return index;
	}
	
	private static bool DrawItem(Rect position, Block block, bool selected, int index) {
		Rect texturePosition = position;
		texturePosition.height = texturePosition.width;
		Rect labelPosition = position;
		labelPosition.yMin += texturePosition.height;
		
		if(selected) EditorGUIUtils.FillRect(labelPosition, new Color( 61/255f, 128/255f, 223/255f ));
		if(block != null) {
			block.DrawPreview(texturePosition);
			GUI.Label(labelPosition, block.GetName());
		} else {
			EditorGUIUtils.FillRect(texturePosition, Color.grey);
			GUI.Label(labelPosition, "Null");
		}
		
		if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition)) {
			Event.current.Use();
			return true;
		}
		return false;
	}
	
	private static void Insert(IList<Block> items, int newIndex, int oldIndex) {
		List<Block> list = new List<Block>(items);
		Block block = list[oldIndex];
		list.RemoveAt(oldIndex);
		list.Insert(newIndex, block);
		
		for(int i=0; i<items.Count; i++) {
			items[i] = list[i];
		}
	}
	
}
