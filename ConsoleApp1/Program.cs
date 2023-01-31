// See https://aka.ms/new-console-template for more information
using IFoundBackend.SqlModels;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

var context = new IFoundContext();

//var requests = (from request in context.TargetPeople.Include(b => b.PostPeople.Select(a=>a.User))
//                select request).ToList();


var queryData = (from request in context.MxFaceIdentities.Include(a => a.Post)                                                      
                 where request.PostId == 12
                 select request).ToList();

Console.WriteLine(queryData);
    