﻿using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Volo.Abp.Reflection;
using Volo.ExtensionMethods.Collections.Generic;

namespace Volo.Abp.Modularity.PlugIns
{
    public class FolderPlugInSource : IPlugInSource
    {
        public string Folder { get; }

        public SearchOption SearchOption { get; set; }

        public FolderPlugInSource([NotNull] string folder, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Check.NotNull(folder, nameof(folder));

            Folder = folder;
            SearchOption = searchOption;
        }

        [NotNull]
        public Type[] GetModules()
        {
            var modules = new List<Type>();

            var assemblies = AssemblyHelper.GetAllAssembliesInFolder(Folder, SearchOption);
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (AbpModule.IsAbpModule(type))
                        {
                            modules.AddIfNotContains(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpException("Could not get module types from assembly: " + assembly.FullName, ex);
                }
            }

            return modules.ToArray();
        }
    }
}