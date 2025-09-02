
// using MasterNet.Domain;
// using MasterNet.Persistence;
// using Microsoft.EntityFrameworkCore;

// using var context = new MasterNetDbContext();
// var  newCourse = new Course 
// {
//     Id = Guid.NewGuid(),
//     Title = "Programming with C#",
//     Description = "The basics of programming",
//     PublicationDate = DateTime.Now
// };

// context.Add(newCourse);
// await context.SaveChangesAsync();

// var courses =  await context.Courses!.ToListAsync();
// foreach(var course in courses)
// {
//     Console.WriteLine($"{course.Id}   {course.Title}");
// }