using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Ambratolm.ScriptGenerator.Utilities
{
    /// <summary>
    /// Selection utility properties and methods.
    /// </summary>
    internal static class SelectionUtility
    {
        /// <summary>
        /// Gets the path of the selected asset in the project window.
        /// </summary>
        /// <returns>A string representing the asset path.</returns>
        public static string SelectedAssetPath => AssetDatabase.GetAssetPath(Selection.activeObject);

        /// <summary>
        /// Gets the path of the currently open asset folder in the project window.
        /// </summary>
        public static string CurrentAssetFolderPath
            => Selection.assetGUIDs.Length == 0 ? "Assets/" : AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

        /// <summary>
        /// Returns true if the selected asset in the project window is a script file, false otherwise.
        /// </summary>
        public static bool SelectedAssetIsScript => Selection.activeObject is MonoScript;
    }

    /// <summary>
    /// File utility properties and methods.
    /// </summary>
    internal static class FileUtility
    {
        /// <summary>
        /// Validates a file path.
        /// <para>
        /// The file path is considered valid if it is not empty, has the specified valid extension,
        /// and the file exists.
        /// </para>
        /// </summary>
        /// <param name="path">The file path to validate.</param>
        /// <param name="validExtension">The expected file extension.</param>
        /// <param name="exception">
        /// The output parameter that stores the exception if the validation fails.
        /// </param>
        /// <returns>True if the file path is valid, false otherwise.</returns>
        public static bool Validate(string path, string validExtension, out Exception exception)
        {
            exception = null;
            if (string.IsNullOrEmpty(path))
                exception = new ArgumentNullException("Empty file path at \"{path}\"");
            else if (!path.EndsWith(validExtension))
                exception = new ArgumentException($"Invalid file extension at \"{path}\". " +
                    $"Extention should be {validExtension}");
            else if (!File.Exists(path))
                exception = new FileNotFoundException($"Invalid file path at \"{path}\". " +
                    $"File not found");
            else
                return true;
            return false;
        }

        /// <summary>
        /// Validates a file path. <br/> The file path is considered valid if it is not empty, has
        /// the specified valid extension, and the file exists.
        /// </summary>
        /// <param name="path">The file path to validate.</param>
        /// <param name="validExtension">The expected file extension.</param>
        /// <returns>True if the file path is valid, false otherwise.</returns>
        public static bool Validate(string path, string validExtension) => Validate(path, validExtension, out _);
    }

    /// <summary>
    /// Type utility properties and methods.
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// Returns an array of types that match the specified criteria.
        /// </summary>
        /// <param name="baseType">The base type to filter by. If null, all types are considered.</param>
        /// <param name="namespaceName">
        /// The namespace to filter by. If null or empty, all namespaces are considered.
        /// </param>
        /// <returns>An array of types that satisfy the conditions.</returns>
        public static Type[] GetTypes(Type baseType = null, string namespaceName = null)
        {
            bool TypeIsValid(Type type)
            {
                bool typeIsValid = true;
                if (baseType is not null) typeIsValid = type.BaseType == baseType;
                if (!string.IsNullOrEmpty(namespaceName)) typeIsValid &= type.Namespace == namespaceName;
                return typeIsValid;
            }
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(TypeIsValid).ToArray();
        }
    }
}