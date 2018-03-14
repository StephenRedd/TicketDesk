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
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicketDesk.Domain;
using TicketDesk.Domain.Model;
using ngWebClientAPI.Models;

using X.PagedList;

namespace ngWebClientAPI.Controllers
{
    //[RoutePrefix("tickets")]
    //[Route("{action=index}")]
    //[TdAuthorize(Roles = "TdInternalUsers,TdHelpDeskUsers,TdAdministrators")]
    public class TicketCenterController : Controller
    {
        private TdDomainContext Context { get; set; }
        public TicketCenterController(TdDomainContext context)
        {
            Context = context;
        }

        [Route("reset-user-lists")]
        public async Task<IPagedList<Ticket>> ResetUserLists()
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            await Context.UserSettingsManager.ResetAllListSettingsForUserAsync(uId);
            var x = await Context.SaveChangesAsync();

            IPagedList<Ticket> ticketList = await Index(null, null);
            return ticketList;

        }

        // GET: TicketCenter
        //[Route("{listName?}/{page:int?}")]
        public async Task<IPagedList<Ticket>> Index(int? page, string listName)
        {

            listName = listName ?? (Context.SecurityProvider.IsTdHelpDeskUser ? "unassigned" : "mytickets");
            var pageNumber = page ?? 1;

            TicketCenterListViewModel viewModel = await TicketCenterListViewModel.GetViewModelAsync(pageNumber, listName, Context, Context.SecurityProvider.CurrentUserId);//new TicketCenterListViewModel(listName, model, Context, User.Identity.GetUserId());
            IPagedList<Ticket> ticketList = viewModel.Tickets;
            return ticketList;
        }

        [Route("pageList/{listName=mytickets}/{page:int?}")]
        public async Task<IPagedList<Ticket>> PageList(int? page, string listName)
        {
            return await GetTicketListPartial(page, listName);
        }

        //[Route("filterList/{listName=opentickets}/{page:int?}")]
        public async Task<IPagedList<Ticket>> FilterList(
            string listName,
            int pageSize,
            string ticketStatus,
            string owner,
            string assignedTo)
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            var userSetting = await Context.UserSettingsManager.GetSettingsForUserAsync(uId);

            var currentListSetting = userSetting.GetUserListSettingByName(listName);

            currentListSetting.ModifyFilterSettings(pageSize, ticketStatus, owner, assignedTo);
            
            await Context.SaveChangesAsync();

            return await GetTicketListPartial(null, listName);

        }

        //[Route("sortList/{listName=opentickets}/{page:int?}")]
        public async Task<IPagedList<Ticket>> SortList(
            int? page,
            string listName,
            string columnName,
            bool isMultiSort = false)
        {
            var uId = Context.SecurityProvider.CurrentUserId;
            var userSetting = await Context.UserSettingsManager.GetSettingsForUserAsync(uId);
            var currentListSetting = userSetting.GetUserListSettingByName(listName);

            var sortCol = currentListSetting.SortColumns.SingleOrDefault(sc => sc.ColumnName == columnName);

            if (isMultiSort)
            {
                if (sortCol != null)// column already in sort, remove from sort
                {
                    if (currentListSetting.SortColumns.Count > 1)//only remove if there are more than one sort
                    {
                        currentListSetting.SortColumns.Remove(sortCol);
                    }
                }
                else// column not in sort, add to sort
                {
                    currentListSetting.SortColumns.Add(new UserTicketListSortColumn(columnName, ColumnSortDirection.Ascending));
                }
            }
            else
            {
                if (sortCol != null)// column already in sort, just flip direction
                {
                    sortCol.SortDirection = (sortCol.SortDirection == ColumnSortDirection.Ascending) ? ColumnSortDirection.Descending : ColumnSortDirection.Ascending;
                }
                else // column not in sort, replace sort with new simple sort for column
                {
                    currentListSetting.SortColumns.Clear();
                    currentListSetting.SortColumns.Add(new UserTicketListSortColumn(columnName, ColumnSortDirection.Ascending));
                }
            }

            await Context.SaveChangesAsync();

            return await GetTicketListPartial(page, listName);
        }



        private async Task<IPagedList<Ticket>> GetTicketListPartial(int? page, string listName)
        {
            var pageNumber = page ?? 1;

            TicketCenterListViewModel viewModel = await TicketCenterListViewModel.GetViewModelAsync(pageNumber, listName, Context, Context.SecurityProvider.CurrentUserId);
            IPagedList<Ticket> tickets = viewModel.Tickets;

            return tickets;

        }
    }
}
