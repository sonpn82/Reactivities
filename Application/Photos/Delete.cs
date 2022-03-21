using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string? Id { get; set; }            
        }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
      private readonly DataContext _context;
      private readonly IPhotoAccessor _photoAccessor;
      private readonly IUserAccessor _userAccessor;

      public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
      {
        _context = context;
        _photoAccessor = photoAccessor;
        _userAccessor = userAccessor;
      }

      public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
      {
        // find the user from current user with photo collection
        var user = await _context.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

        if (user == null) return null!;

        // find the photo in photo collection with id
        // do not need async here because image is already in memory
        var photo = user.Photos!.FirstOrDefault(x => x.Id == request.Id);

        if (photo == null) return null!;

        // if photo is main photo then it can not be deleted
        if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo");

        // delete the photo in Cloudinary using PhotoAccessor
        var result = await _photoAccessor.DeletePhoto(photo.Id!);

        if (result == null) return Result<Unit>.Failure("Problem deleting photo from Cloudinary");

        // remove photo in our database
        user.Photos!.Remove(photo);

        // save database changed
        var success = await _context.SaveChangesAsync() > 0;

        if (success) return Result<Unit>.Success(Unit.Value);

        return Result<Unit>.Failure("Problem deleting photo from API");

      }
    }
  }
}