using Ambratolm.ScriptGenerator.Utilities;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Ambratolm.ScriptGenerator.Utilities.SelectionUtility;

namespace Ambratolm.ScriptGenerator
{
    /// <summary>
    /// Generates C# classes from template assets.
    /// </summary>
    internal sealed class ClassGenerator : AssetModificationProcessor
    {
        private const string _templateExtension = ".template.cs";
        private const string _assetMenuItemPath = "Assets/Generate C# Class";
        private static string _templateFilePath;
        private static string _templateFileName;

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Menu item function. It generates a class file from the selected class template asset.
        /// </summary>
        [MenuItem(_assetMenuItemPath)]
        public static void OnGenerate() => Generate();

        /// <summary>
        /// Menu item validation function. It validates the selected asset.
        /// <para>
        /// Called before invoking the menu item function with the same itemName on the MenuItem attribute.
        /// </para>
        /// </summary>
        /// <returns>A boolean value that indicates whether the validation succeeded or not.</returns>
        [MenuItem(_assetMenuItemPath, isValidateFunction: true)]
        private static bool OnValidate() => Validate(throwException: false);

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// Generates a C# class file from a template and saves it to the output location.
        /// </summary>
        private static void Generate()
        {
            Validate();
            Type templateType = GetClassTemplateType(out string templateText);
            ClassTemplate template = Activator.CreateInstance(templateType, templateText) as ClassTemplate;
            string className = template.HasClass ? template.ClassName : "_";
            string fileName = $"{className}.cs";
            string filePath = _templateFilePath.Replace(_templateFileName, fileName);
            File.WriteAllText(filePath, contents: template.GenerateClassCode());
            UnityEngine.Object generatedAsset = AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityEngine.Object));
            Debug.Log($"\"{className}\" class generated at \"{filePath}\"", generatedAsset);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Validates the selected asset.
        /// <para>
        /// The asset is considered valid if it is a class template script that has the template extension.
        /// </para>
        /// </summary>
        /// <param name="throwException">Whether to throw an exception if the validation fails.</param>
        /// <returns>True if the asset is valid, false otherwise.</returns>
        private static bool Validate(bool throwException = true)
        {
            _templateFilePath = SelectedAssetPath;
            _templateFileName = Path.GetFileName(_templateFilePath);
            bool assetIsValid = SelectedAssetIsScript;
            if (throwException && !assetIsValid) throw new InvalidDataException($"Invalid asset type at \"{_templateFilePath}\". " +
                $"\"{_templateFileName}\" asset should be a script.");
            assetIsValid &= FileUtility.Validate(_templateFilePath, _templateExtension, out Exception exception);
            if (throwException && !assetIsValid)
                throw exception;
            return assetIsValid;
        }

        /// <summary>
        /// Gets the type of the ClassTemplate class defined in the selected script and its text.
        /// </summary>
        /// <param name="classTemplateText">The text of the class template script.</param>
        /// <returns>The class template type.</returns>
        /// <exception cref="InvalidDataException">
        /// Thrown when the script is invalid or does not inherit from ClassTemplate.
        /// </exception>
        private static Type GetClassTemplateType(out string classTemplateText)
        {
            MonoScript script = Selection.activeObject as MonoScript;
            Type classTemplateType = script.GetClass();
            if (classTemplateType is null) throw new InvalidDataException($"Invalid script at \"{_templateFilePath}\". " +
                $"Script doesn't implement any class. It should implement a class that inherits from \"{nameof(ClassTemplate)}\"");
            if (!classTemplateType.IsSubclassOf(typeof(ClassTemplate))) throw new InvalidDataException($"Invalid script at \"{_templateFilePath}\". " +
                $"Implemented \"{classTemplateType.Name}\" class should inherit from \"{nameof(ClassTemplate)}\".");
            classTemplateText = script.text;
            return classTemplateType;
        }
    }
}