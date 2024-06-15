using CrudMVC.Models;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrudMVC.Models;
using System.Data.Entity;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

public class UserProfilesController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    public ActionResult Index()
    {
        return View(db.UserProfiles.ToList());
    }

    public ActionResult List()
    {
        var userProfiles = db.UserProfiles.ToList();
        return PartialView("_UserProfileList", userProfiles);
    }

    [HttpPost]
    public ActionResult Delete(int id)
    {
        UserProfile userProfile = db.UserProfiles.Find(id);
        db.UserProfiles.Remove(userProfile);
        db.SaveChanges();
        return new HttpStatusCodeResult(HttpStatusCode.OK);
    }

    public ActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public ActionResult UploadFile(HttpPostedFileBase file)
    {
        if (file != null && file.ContentLength > 0)
        {
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                file.SaveAs(path);
                return Json("File uploaded successfully!");
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }
        return Json("No file selected.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(UserProfile userProfile, HttpPostedFileBase profilePhoto)
    {
        if (ModelState.IsValid)
        {
            // Check if the email already exists
            if (IsEmailUnique(userProfile.Email))
            {
                // Process file upload if profile photo is provided
                if (profilePhoto != null && profilePhoto.ContentLength > 0)
                {
                    var uploadsDir = Server.MapPath("~/Uploads");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var fileName = Path.GetFileName(profilePhoto.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);
                    profilePhoto.SaveAs(filePath);

                    userProfile.ProfilePhotoPath = fileName;
                }

                // Add the new UserProfile to the database
                db.UserProfiles.Add(userProfile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("Email", "Email address already exists.");
            }
        }

        return View(userProfile);
    }

    private bool IsEmailUnique(string email)
    {
        // Check if there is no existing user with the same email address
        return !db.UserProfiles.Any(u => u.Email == email);
    }


    // GET: UserProfiles/Edit/5
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        UserProfile userProfile = db.UserProfiles.Find(id);
        if (userProfile == null)
        {
            return HttpNotFound();
        }
        return View(userProfile);
    }

    // POST: UserProfiles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Name,Email,Country,State,ProfilePhotoPath")] UserProfile userProfile, HttpPostedFileBase ProfilePhotoPath)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Check if a new profile photo was uploaded
                if (ProfilePhotoPath != null && ProfilePhotoPath.ContentLength > 0)
                {
                    // Define the upload directory (you might want to change this path as per your application's structure)
                    var uploadsDir = Server.MapPath("~/Uploads");

                    // If the directory doesn't exist, create it
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Generate a unique file name to prevent conflicts
                    var fileName = Path.GetFileName(ProfilePhotoPath.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    // Save the uploaded file to the server
                    ProfilePhotoPath.SaveAs(filePath);

                    // Update the ProfilePhotoPath in the userProfile object
                    userProfile.ProfilePhotoPath = Path.Combine("~/Uploads", fileName); // Assuming "~/Uploads" is accessible via URL
                }

                // Update the userProfile in the database
                db.Entry(userProfile).State = EntityState.Modified;
                db.SaveChanges();

                // Redirect to the Index action upon successful save
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during file upload or database save
                ModelState.AddModelError("", "Error occurred while saving your profile. Please try again.");
            }
        }

        // If ModelState is not valid or an exception occurred, return to the Edit view with errors
        return View(userProfile);
    }




    // Implement Edit actions similarly with AJAX
}
