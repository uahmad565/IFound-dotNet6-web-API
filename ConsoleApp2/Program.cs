// See https://aka.ms/new-console-template for more information
using IFoundBackend.SqlModels;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Hello, World!");

var dbContext= new IFoundContext();

var requests = (from request in dbContext.MxFaceIdentities.Include(a => a.Post.Image)
                                                          .Include(c => c.Post.Person)
                                                          .Include(d => d.Post.Status) select request);


var req1=(from x in dbContext.PostStatuses
                            .Include(postStatus => postStatus.PostPeople)
                                .ThenInclude(PostPerson => PostPerson.MxFaceIdentities)
                            .Include(PostStatus=>PostStatus.PostPeople)
                                .ThenInclude(PostPerson=>PostPerson.Person)
                            .Include(postStatus => postStatus.PostPeople)
                                .ThenInclude(PostPerson => PostPerson.Image)
                       
          select x).ToList();

Console.WriteLine(requests);