﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs.TabVersions;
using DotNetNuke.Framework;

namespace DotNetNuke.Entities.Tabs
{
    public class TabChangeTracker : ServiceLocator<ITabChangeTracker, TabChangeTracker>, ITabChangeTracker
    {
        public void TrackModuleAddition(ModuleInfo module, int moduleVersion, int userId)
        {
            if (TabChangeSettings.Instance.IsChangeControlEnabled(module.PortalID, module.TabID))
            {
                TabVersionTracker.Instance.TrackModuleAddition(module, moduleVersion, userId);
            }
            if (TabWorkflowSettings.Instance.IsWorkflowEnabled(module.PortalID, module.TabID))
            {
                TabWorkflowTracker.Instance.TrackModuleAddition(module, moduleVersion, userId);
            }
        }

        public void TrackModuleModification(ModuleInfo module, int moduleVersion, int userId)
        {
            if (SharedModuleController.Instance.IsSharedModule(module) && moduleVersion != Null.NullInteger)
            {
                throw SharedModuleController.Instance.GetModuleDoesNotBelongToPagException();
            }

            if (TabChangeSettings.Instance.IsChangeControlEnabled(module.PortalID, module.TabID))
            {
                TabVersionTracker.Instance.TrackModuleModification(module, moduleVersion, userId);
            }

            if (TabWorkflowSettings.Instance.IsWorkflowEnabled(module.PortalID, module.TabID))
            {
                TabWorkflowTracker.Instance.TrackModuleModification(module, moduleVersion, userId);
            }
        }
        
        public void TrackModuleDeletion(ModuleInfo module, int moduleVersion, int userId)
        {
            if (TabChangeSettings.Instance.IsChangeControlEnabled(module.PortalID, module.TabID))
            {
                TabVersionTracker.Instance.TrackModuleDeletion(module, moduleVersion, userId);
            }
            if (TabWorkflowSettings.Instance.IsWorkflowEnabled(module.PortalID, module.TabID))
            {
                TabWorkflowTracker.Instance.TrackModuleDeletion(module, moduleVersion, userId);
            }
        }
        
        protected override Func<ITabChangeTracker> GetFactory()
        {
            return () => new TabChangeTracker();
        }
    }
}
