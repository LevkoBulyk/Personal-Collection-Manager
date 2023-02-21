# Personal-Collection-Manager

The most recent version of the project is deployed [here](http://volbog-001-site1.atempurl.com/), and will be awailable untill 30th of March, 2023.
Please, be informed, that the deployed version is still being developed, therefore, not all functionality may work properly.

.NET: C#, MVC, EntityFramework, SQL Server
Front: JavaSCript, html Razor pages

You need to implement a Web-app for personal collections managements (books, stamps, medals, whiskeys, etc. - it's called items in the text below).

Nonauthenticated used have read-only access (they can use search, however cannot create collections and items, create comments or likes).

Authenticated users have access to everythng except "admin area".

Admin area give administators abilities to manage users (view, block, remove, add to/remove from admin role). Admin see every user page and collection as its creator (e.g. admin can edit collection or add an item from users page). 

Collection can be managed (edit/add/remove) only by the owner (creator) or admin.

App should support registration and authentication.

Every page provides access to full-text search over whole site (results are represented as item list, e.g., if some text is found in comment, the results page diplays link to the corresponding item page).

Every user has a personal page, which allow to manage list of own collections (add/remove/edit) and allow to open page dedicated to given collection (that page contains table with filters and sorting as well as actions to cretae/remove/edit item).
Each collection consists of: name, short description with markdown formatting, "topic" (from fixed set, e.g., Alcohol|Books|Cola Cans|...), optional image (stored in the cloud, upload with the help of drag-n-drop). Also collection allows to define custom fields, which will be filled for each item in this collection. There 3 fixed field (id, name, tags) but it's also possible to add dynamically something from the following - 3 number filed, 3 string filed, 3 multiline text fields, 3 dates, 3 boolean checkboxes. For example, the user may define that each item in his/her collection contains (in addition to id, name and tags) string field "Author", text field "Comment" and date-field "Publication year". Multiline text fields should support markdown. Each custom filed should have name )which will be displayed in the item form). Custom fields are shown in the item list on the collection page (with the sorting and filtering support).

Each item has tags (user enters several tags with autocompletion, when user starts to enter tag, you show the dropdown with the words entered on the site before by all users).

On the main page: last added items, biggest collections, clickable tag could (when users click on the tag, he/she gets and seach result page - it can be the same view).

When item is opened for reading by author or opened by other user, there are comments at the bottom. Comments are linear, users don't put comments to other comments, only to the end of the comment list. You have to implement automatic fetching of new comments - if I open page with comments and somebody wrote a new comment, it should appear on my page (it's possible to have 2-5 seconds delay).

Item has likes (no more than 1 from user per item).
Your app should support 2 languages: English and Russian (you may replace Russina with any other) - only UI is translated, not content. Your app should support two visual themes (skins) - dark and light. User may change languge or theme and the choice is stored for the user.

Require: Bootstrap (or any other CSS-framework), different resolution support (including phones), ORM for data access, engine for full-text search (external or embedded in DB, but NOT THE FULL SCAN by SELECTs).

Optional requirements (if everything else is done, for 10/10 maker):
* Authentication via social networks.

* Add custom fields with the type "one of many" (dropdown) with ability to define custom list of values.

* Arbitraty number of custom fields, not just up to 3 for every of 5 types.

* Export collection to CSV-file.
IMPORTANT NOTE: do not copy any code from code heaps. IT’S MUCH BETTER TO DO LESS, BUT UNDERSTAND COMPLETELY WHAT DID YOU WRITE EXACTLY. I’m dead serious — we will ask you mo modify your code on the fly, add something or change something, will ask you questions and will check your ability to work with your project code. It’s more important than number of implemented requirements. Your supervisor saw a lot of similar projects and know probably most of the available sh..t on this topic in Internet. Do not copy. Use libraries as much as possible. But don’t copy. 
You have to use ready components, libraries, controls. E.g. use ready-to-use control to render markdown or ready-to-use control to upload images with drag-n-drop or ready-to-use control to enter tags or ready-to-use control to render tag cloud, etc.
