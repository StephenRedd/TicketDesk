﻿// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//
// This file is distributed under the terms of the Microsoft Public 
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use. 
//
// For any distribution that contains code from this file, this notice of 
// attribution must remain intact, and a copy of the license must be 
// provided to the recipient.

using System;
using ngWebClientAPI.Models;
using TicketDesk.Domain.Model;
//using Microsoft.AspNet.Identity;
//using TicketDesk.Web.Identity.Model;

namespace ngWebClientAPI
{

    public class TicketDeskUserManager
    {
        public bool IsTdHelpDeskUser(CPUUser user)
        {
            bool IsTdHelpDeskUser = true;
            
            /*Need to change this method.*/
            return IsTdHelpDeskUser;
        }

        public bool IsTdInternalUser(CPUUser user)
        {
            /*Need to change this method.*/
            bool IsTdInternalUser = true;
          
            return IsTdInternalUser;
        }

        public bool IsTdAdministrator(CPUUser user)
        {
            /*Need to change this method.*/
            bool IsTdAministrator = true;
            
            return IsTdAministrator;
        }

        public bool IsTdPendingUser(CPUUser user)
        {
            /*Need to change this method.*/
            bool IsTdPendingUser = false;
            return IsTdPendingUser;
        }
    }
}