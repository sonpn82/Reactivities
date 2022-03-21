using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile? File {get; set;}  // file to be added          
        }

    public class Handler : IRequestHandler<Command, Result<Photo>>
    {
      private readonly DataContext _context;  // database
      private readonly IPhotoAccessor _photoAccessor;  // access photo on cloudinary
      private readonly IUserAccessor _userAccessor;  // get current user

      public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
      {
        _context = context;
        _photoAccessor = photoAccessor;
        _userAccessor = userAccessor;
      }

      public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
      {
        // find the current user with photos collection
        var user = await _context.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        
        if (user == null) return null!;

        // upload photo to Cloudinary
        var photoUploadResult = await _photoAccessor.AddPhoto(request.File!);

        // Get back the Url and Id of uploaded photo
        var photo = new Photo{
            Url = photoUploadResult.Url,
            Id = photoUploadResult.PublicId
        };

        // if no photo in collection is main then set it to main
        if (!user.Photos!.Any(x => x.IsMain)) photo.IsMain = true;

        // add the photo to database 
        user.Photos!.Add(photo);

        // save change to database
        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Result<Photo>.Success(photo);

        return Result<Photo>.Failure("Problem adding photo");
      }
    }
  }
}