# Changelog

## [Unreleased]

## [0.2.1] - 2023-10-12

### Added

- New WebContext database to start migrating to simpler architecture and principle

### Changed

- Change Canvas related UseCase logic back to Web to reduce architecture layering

### Log

I already notice some of the benefits of being way more flexible when not using DDD principles.
Also, what Jimmy Bogard said regarding having a vertical slice in a page file is immediately noticable. 
There is no apparent disadvantage of having the code not in usecase classes. 
On the contrary, the test naming becomes obvious AND I can test the web project as well.