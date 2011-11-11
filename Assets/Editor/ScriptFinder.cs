using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.IO;


/// <summary>
/// Finds MonoBehaviour that are not attached to any objects in any scene.
/// </summary>
/// <remarks>
/// This tool does not take into account prefabs or components that are instantiated at runtime. Use with caution.
/// </remarks>
public sealed class ScriptFinder : EditorWindow
{
	
	// Find MonoBehaviours which are not attached, but are still being called by other code.
	// (they don't need to be monobehaviours in this case)
	
	#region Window Setup

	private static ScriptFinder window;

	[MenuItem("Custom/Find Unused Scripts")]
	static void Init ()
	{
		ScriptFinder window = (ScriptFinder)EditorWindow.GetWindow (typeof(ScriptFinder), true, "Script Finder");
	}

	#endregion

	
	
	private class ScriptReference {
		public MonoScript script;
		/// <summary>
		/// Prefabs containing the script.
		/// </summary>
		public UnityEngine.Object[] prefabs;
		/// <summary>
		/// Scene files containing the script.
		/// </summary>
		public UnityEngine.Object[] scenes;
		/// <summary>
		/// Game objects in the current scene containing the script.
		/// </summary>
		public UnityEngine.Object[] gameObjects;
	}
	
	
	
	/// <summary>
	/// Get MonoScripts which are used in the given scene or prefab.
	/// </summary>
	private List<MonoScript> GetScriptDependenciesForAsset (UnityEngine.Object obj)
	{
		List<MonoScript> scripts = new List<MonoScript> ();
		foreach (var s in EditorUtility.CollectDependencies (Selection.objects)) {
			if (s as MonoScript) {
				scripts.Add ((MonoScript)s);
			}
		}
		return scripts;
	}
	
	
	
	/// <summary>
	/// Get all scene files contained in the project.
	/// </summary>
	private List<UnityEngine.Object> GetAllSceneAssets ()
	{
		return GetAllAssetsOfTypeWithExtension<UnityEngine.Object> (".unity");
	}
	
	
	/// <summary>
	/// Get all prefabs contained in the project.
	/// </summary>
	private List<UnityEngine.Object> GetAllPrefabAssets ()
	{
		return GetAllAssetsOfTypeWithExtension<UnityEngine.Object> (".prefab");
	}
	
	
	/// <summary>
	/// Get all asset files in the project which match the given extension (including the dot)
	/// </summary>
	private List<T> GetAllAssetsOfTypeWithExtension<T> (string extension)
		where T : UnityEngine.Object
	{
		List<T> objs = new List<T> ();
		foreach (string path in AssetDatabase.GetAllAssetPaths ()) {
			if (Path.GetExtension (path) != extension)
				continue;
			T asset = AssetDatabase.LoadAssetAtPath (path, typeof(T)) as T;
			if (asset != null)
				objs.Add (asset);
		}
		return objs;
	}
	
	
	/// <summary>
	/// Find all MonoBehaviour files contained in the project.
	/// </summary>
	private static List<MonoScript> FindAllMonoBehaviourScriptsInProject ()
	{
		List<MonoScript> scripts = new List<MonoScript> ();
		foreach (var obj in FindObjectsOfTypeIncludingAssets (typeof(MonoScript))) {
			if (obj as MonoScript) {
				MonoScript script = (MonoScript)obj;
				if (script.GetClass ().IsSubclassOf (typeof(MonoBehaviour))) {
					scripts.Add (script);
				}
			}
		}
		return scripts;
	}
	
	
	
	
	
	
	// When displaying scenes that use a particular script, click to highlight the scene file.
	// Double click to open and highlight the objects that use the script. Also expands
	// The view 
	
	// Auto refresh when deleting or changing files. Is there a callback for this? May have to use my Watcher
	
	
	
	
	void OnGUI ()
	{	
		if (GUILayout.Button ("Show all MonoBehaviour Scripts")) {
			foreach (MonoScript script in FindAllMonoBehaviourScriptsInProject ()) {
				Debug.Log (script.GetClass ());
			}
		}
		
		if (GUILayout.Button ("Show script dependencies for selected")) {
			foreach (MonoScript script in GetScriptDependenciesForAsset (Selection.activeObject)) {
				Debug.Log (script.GetClass ());
			}
		}
		
		
		if (GUILayout.Button ("Find Selected Script in Assets")) {
			
		}

		
		
//		foreach (var script in unusedMonoScripts) {
//			GUIStyle style = new GUIStyle ("button");
//			style.alignment = TextAnchor.MiddleLeft;
//			GUILayout.BeginHorizontal ();
//			{
//				if (GUILayout.Button (script.name, style)) {
//					EditorGUIUtility.PingObject (script);
//				}
//				if (GUILayout.Button ("Open", GUILayout.Width (50))) {
//					EditorUtility.OpenWithDefaultApp (AssetDatabase.GetAssetPath (script));
//				}
//			}
//			GUILayout.EndHorizontal ();
//		}
	}
	
	
	
	private List<MonoScript> unusedMonoScripts = new List<MonoScript> ();
	
	
//	private static HashSet<System.Type> FindUnusedMonoBehaviours ()
//	{
//		HashSet<System.Type> existingBehaviours = FindMonoBehaviours ();
//		HashSet<System.Type> usedBehaviours = new HashSet<System.Type> ();
//		
//		// Iterate through all scenes that exist in the project
//		foreach (string s in FindAllScenes ()) {
//			EditorApplication.OpenScene (s);
//			// Find all MonoBehaviours in each scene
//			foreach (var type in existingBehaviours) {
//				// Build a set of MonoBehaviours that are in use
//				if (CurrentSceneContainsMonoBehaviour (type)) {
//					if (!usedBehaviours.Contains (type)) {
//						usedBehaviours.Add (type);
//					}
//				}
//			}
//		}
//		
//		// Remove all used MonoBehaviours from the set of all existing MonoBehaviours
//		existingBehaviours.ExceptWith (usedBehaviours);
//		return existingBehaviours;
//	}
	
	
}
