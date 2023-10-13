# Changelog

## [Unreleased]

## [0.2.1] - 2023-10-12

### Added

- New WebContext database to start migrating to simpler architecture and principle

### Changed

- Change Canvas related UseCase logic back to Web to reduce architecture layering
- Change StudentGroup to Adventure
- Deleting an adventure can now be done in the overview page inline! (not really, but really!)
- Adding a custom adventure can now be done on the overview page itself.

### Fixed

### Log

I already notice some of the benefits of being way more flexible when not using DDD principles.
Also, what Jimmy Bogard said regarding having a vertical slice in a page file is immediately noticable. 
There is no apparent disadvantage of having the code not in usecase classes. 
On the contrary, the test naming becomes obvious AND I can test the web project as well.

So htmx really keeps amazing me. 
With just some simple code that doesn't take too much knowledge you can create SPA like behavior, without all the JS fluff.

Also, there's a really nice nuget package that let's me use htmx easily on the backend.
I would really like to try out the taghelpers too though...