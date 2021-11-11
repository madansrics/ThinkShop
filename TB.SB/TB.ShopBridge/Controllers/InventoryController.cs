using System;
using System.Collections.Generic;
using System.Linq;
using TB.ShopBridge.Models;
using System.Web.Mvc;

namespace TB.ShopBridge.Controllers
{
    public class InventoryController : Controller
    {


        /// <summary>
        /// Used for viewpage 
        /// </summary>
        /// <returns></returns>
        public ActionResult InventoryView()
        {
            Inventory objinv = new Inventory();
            TempData.Remove("ItemId");
            return View(objinv);
        }


        /// <summary>
        /// Used for edit the item record
        /// </summary>
        /// <param name="itid">  item id (pkid)</param>
        /// <returns></returns>
        public ActionResult ItemEdit(int itid)
        {
            TempData["ItemId"] = itid;//Item id
            TempData.Keep("ItemId");
            return Json(JsonRequestBehavior.AllowGet);
        }






        /// <summary>
        /// Get method to fetch the Details
        /// </summary>

        [HttpGet]

        public ActionResult InventoryDetails()
        {
            Inventory objinv = new Inventory();
            try
            {
                if (TempData["ItemId"] != null)
                {
                    objinv.ItemId = Convert.ToInt32(TempData["ItemId"]);
                    TempData.Keep("ItemId");
                    objinv.GetInventoryDetails(objinv);

                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return View(objinv);
        }




        /// <summary>
        /// This is post method to save or update the Details
        /// </summary>
        /// <param name="objinv"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InventoryDetails(Inventory objinv)
        {
            try
            {
                objinv.SaveInventoryDetails(objinv);
            }
            catch (Exception ex)
            {
                objinv.Message = "Please Try again later..Something went wrong!!!";
                objinv.Status = -1;
                throw ex;
            }
            return View(objinv);
        }





        /// <summary>
        /// Used for geting the saved records for view
        /// </summary>
        /// <param name="objinv"></param>
        /// <returns></returns>
        public ActionResult GetSavedDetails()
        {
            Inventory objinv = new Inventory();
            try
            {
                objinv.GetInventoryDetails(objinv);
                return Json(new { data = objinv.listSavedInventory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        /// <summary>
        /// Used for delete the item record
        /// </summary>
        /// <param name="itid">it is item id (pkid)</param>
        /// <returns></returns>
        public ActionResult RemoveItem(int itid)
        {
            Inventory objinv = new Inventory();
            try
            {
                objinv.ItemId = itid;
                objinv.RemoveItem(objinv);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }








    }
}