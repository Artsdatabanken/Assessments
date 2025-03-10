# Assessments.Web - also known as the  web app

## How to

### Make areas in the assessment page collapsible

Follow the recipe found in top of the file "Assessments.Web/wwwroot/js/accordions.js". 

### Adding a filter to the list view

#### Rødlista for arter 2021

Hard coded in _Filters.cshtml partials file.

#### Alien Species 2023

- Add the filter object to FilterItemsHelpers.cs

- In Models/AlienSpeciesListViewModels.cs do:
	- Declare the field in the view model. These should be alphabetically sorted, like most things should.
	- Add the name of the field to FilterParameters.

- In ListPartials/_Filters.cshtml do:
	- Declare the filter object in ListPartials/_Filters.cshtml as "All<filter name>"
	- Call the MakeFilterChips function. You will find all these calls grouped together in the file.
	- Call the MakeFilterGroup function. You will find all these calls grouped together in the file.

- In Infrastructure/AlienSpecies/FilterHelpers.cs do:
	- Add a case to the GetActiveFilters function
	- Add filter to the counter in the GetActiveSelectionCount function
	- Add the filter to the selection list in the GetActiveSelectionElement function
	- Add the displayed name of the filter to SearchAndFilterNames

- In wwwroot/css/filter.css do:
	- Add `.only_js input[type=checkbox]:not(:checked)#show_<filter name> ~ .filter_<filter name>` to the group, or sub group of filters. Search for comment "Hide the filters initially" to find the location.

- In wwwroot/js/filter.js do:
	- Add the filter's checkbox id to the list "handleFirstTimeIds" if you want it to open by default.

- In the file Infrastructure/AlienSpecies/QueryHelpers.cs, apply the filter to the query results list.

### Adding a section with tabs

#### All lists

- Redlist species 2021 already has three special tab sections, but adding new ones will be done in the same generic way as the rest. 

- You can find thorough information on how to add a new tabbable section in the assessment pages in the top of the file "wwwroot/js/assessmentTabs.js".

Remember to rebuild the solution before testing.
