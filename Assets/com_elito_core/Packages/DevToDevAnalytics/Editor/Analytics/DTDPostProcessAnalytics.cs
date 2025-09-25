﻿#if UNITY_IOS
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace DevToDev.Analytics.Editor
{
    public static class DTDPostProcessAnalytics
    {
        private const string UNITY_ANALYTICS_NAME = "DTDAnalyticsiOSUnity.xcframework";
        private const string PACKAGE_NAME = "com.devtodev.sdk.analytics";

        [PostProcessBuild(98)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.iOS)
            {
                OnPostProcessBuildIOS(target, pathToBuiltProject);
            }
        }

        private static void OnPostProcessBuildIOS(BuildTarget target, string pathToBuiltProject)
        {
            var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var project = new UnityEditor.iOS.Xcode.PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));

#if UNITY_2019_3_OR_NEWER
            var mainGuid = project.GetUnityMainTargetGuid();
            var targetGuid = project.GetUnityFrameworkTargetGuid();
            //Fix for
            //https://forum.unity.com/threads/2019-3-validation-on-upload-to-store-gives-unityframework-framework-contains-disallowed-file.751112/
            project.SetBuildProperty(mainGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
#else
            var name = UnityEditor.iOS.Xcode.PBXProject.GetUnityTargetName();
            var targetGuid = project.TargetGuidByName(name);
#endif
#if DTD_COPPA
            project.AddFrameworkToProject(targetGuid, "AdSupport.framework", true);
            project.AddFrameworkToProject(targetGuid, "AppTrackingTransparency.framework", true);
#endif

            var frameworkAbsolutePath = Path.Combine("Plugins", "DevToDev", "Analytics", "IOS");
            frameworkAbsolutePath = Path.Combine(Application.dataPath, frameworkAbsolutePath, UNITY_ANALYTICS_NAME);
            if (!Directory.Exists(frameworkAbsolutePath))
            {
                frameworkAbsolutePath = Path.Combine("Packages", PACKAGE_NAME, "Plugins", "Analytics", "IOS");
                frameworkAbsolutePath = Path.GetFullPath(Path.Combine(frameworkAbsolutePath, UNITY_ANALYTICS_NAME));
            }

            AddAnalyticsFramework(pathToBuiltProject, project, targetGuid, frameworkAbsolutePath);
            File.WriteAllText(projectPath, project.WriteToString());
        }

        private static void AddAnalyticsFramework(string projectPath, UnityEditor.iOS.Xcode.PBXProject project,
            string targetGuid,
            string frameworkPath)
        {
            bool xcFrameworkSupported;
            try
            {
                var version = GetVersion(Application.unityVersion);
                xcFrameworkSupported = IsXcFrameworkSupported(version[0], version[1], version[2]);
            }
            catch (Exception)
            {

                xcFrameworkSupported = false;
            }
            if (!xcFrameworkSupported)
            {
                var destinationFrameworkFilePath = Path.Combine(projectPath, "Frameworks", UNITY_ANALYTICS_NAME);
                // Copy framework
                DirectoryCopy(frameworkPath, destinationFrameworkFilePath, true);
                // Add declaration to .xcodeproj.
                var fileInBuild =
                    project.AddFile($"Frameworks/{UNITY_ANALYTICS_NAME}", $"Frameworks/{UNITY_ANALYTICS_NAME}");
                string targetBuildPhaseGuid = project.AddFrameworksBuildPhase(targetGuid);
                project.AddFileToBuildSection(targetGuid, targetBuildPhaseGuid, fileInBuild);
            }

            project.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/Frameworks");
            var defaultProperties = project.GetBuildPropertyForAnyConfig(targetGuid, "LD_RUNPATH_SEARCH_PATHS");
            project.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "/usr/lib/swift");
            if (defaultProperties != null)
            {
                foreach (var p in defaultProperties.Split(' '))
                {
                    project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", p);
                }
            }

            project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
            var defaultLibraryProperties = project.GetBuildPropertyForAnyConfig(targetGuid, "LIBRARY_SEARCH_PATHS");
            project.SetBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", "$(SDKROOT)/usr/lib/swift");
            project.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS",
                "$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)");
            project.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS",
                "$(TOOLCHAIN_DIR)/usr/lib/swift-5.0/$(PLATFORM_NAME)");

            var properties = defaultLibraryProperties.Split(' ');
            foreach (var property in properties)
            {
                project.AddBuildProperty(targetGuid, "LIBRARY_SEARCH_PATHS", property);
            }

#if !UNITY_2019_3_OR_NEWER
            project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
#endif
        }

        /// <summary>
        /// Recursively directory copy.
        /// </summary>
        /// <param name="sourceDirName">Source dit path.</param>
        /// <param name="destDirName">Destination dir path.</param>
        /// <param name="copySubDirs">Copy sub directories?</param>
        /// <param name="specificExtensions">
        /// If not empty will be copied only files with specific extensions.
        /// Example of extension: ".dll"
        /// </param>
        // Source: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,
            params string[] specificExtensions)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: {sourceDirName}");
            }

            // Delete destination directory if exist.
            if (Directory.Exists(destDirName))
            {
                Directory.Delete(destDirName, true);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = null;
            if (specificExtensions != null && specificExtensions.Length > 0)
            {
                // Get the files of the specific extensions in the directory.
                files = dir.GetFiles().Where(x => specificExtensions.Contains(x.Extension)).ToArray();
            }
            else
            {
                // Get the all files in the directory.
                files = dir.GetFiles();
            }

            files = files.Where(x => x.Extension != ".meta").Where(x => x.Extension != ".DS_Store").ToArray();
            // Copy files to the new location
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }


            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs, specificExtensions);
                }
            }
        }
        private static int[] GetVersion(string version)
        {
            var match = Regex.Match(version, @"^(\d+)\.(\d+)\.(\d+)([a-z]?)(\d*)$", RegexOptions.IgnoreCase);
            int major, minor, patch;
            if (match.Success)
            {
                major = int.Parse(match.Groups[1].Value);
                minor = int.Parse(match.Groups[2].Value);
                patch = int.Parse(match.Groups[3].Value);
            }
            else
            {
                string cleanedVersion = Regex.Replace(Application.unityVersion, "[^0-9.]", "");
                major = int.Parse(cleanedVersion.Split('.')[0]);
                minor = int.Parse(cleanedVersion.Split('.')[1]);
                patch = int.Parse(cleanedVersion.Split('.')[2]);
            }

            return new int[] { major, minor, patch };
        }

        private static bool IsXcFrameworkSupported(int major, int minor, int patch)
        {
            switch (major)
            {
                case 6000: return true;
                case 2021: return (minor >= 3 && patch >= 37);
                case 2022: return (minor >= 3 && patch >= 23);
                case 2023: return (minor >= 2 && patch >= 18);
            }

            return false;
        }
    }
}
#endif