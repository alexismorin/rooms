using UnityEditor;
using UnityEngine;

public class ModelProcessor : AssetPostprocessor {
    void OnPreprocessModel() {

        ModelImporter modelImporter = assetImporter as ModelImporter;

        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
        // modelImporter.generateSecondaryUV = true;

    //   if (!assetPath.Contains("@")) {
    //        modelImporter.importAnimation = false;
     //   }
    }
}