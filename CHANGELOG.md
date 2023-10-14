# Changelog

## [Unreleased]

## [0.2.1] - 2023-10-12

### Added

- New WebContext database to start migrating to simpler architecture and principle
- `.editorconfig` to **try** and get rider settings into vscode
- Deleting adventures from the adventure index page (with nice transitions).
- Transitions between actions on the Adventures overview page.

### Changed

- Change Canvas related UseCase logic back to Web to reduce architecture layering
- Change StudentGroup to Adventure
- Deleting an adventure can now be done in the overview page inline! (not really, but really!)
- Adding and editing a adventure can now be done on the overview page itself.
- Made people search absolutely JavaScript free (0 lines!). Also no more dependency on popper.js

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
Update: Removed the nuget package after learning a bit more of htmx.
To be honest, the current implementation of adding the xsrf token and the way I check the hx-trigger to determine the partial return is way better (thank you htmx book).

I changed the people (former student) search from alpinejs and popperjs to just simple htmx and css.
Now there are 0 lines of javascript, instead of 52. Still impressed 🙌.
Also, using purely razor syntax and c# to write the implementation is simply amazing.

Architecture wise, especially when working alone, it's amazing to just work with plain simple anemic models and have the business logic inside the page models. Look at the [query I wrote for searching](./StudentProgress.Web/Pages/People/Parts/Search.cshtml.cs).
I can simply select columns I want, put it in the same model and send it to the client without any hassle of converting.
We'll see down the line how hard it's gonna bite me in the butt though.

So I'm now removing the `Parts` directory from the project already.
The reason is that when main page uses a partial that another "partial" page also uses, you will get relative path issues.
It makes sense though, you can still see when a page is a partial, because of the `Layout = null;` statement.
In the meantime, actual partials have a nice _ prefix.

I'm becoming more aware of the power of partials, muhaha.
No really, it's really cool how much you can achieve without writing javascript and add some flair to a simple crud page.