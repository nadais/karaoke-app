﻿Feature: UploadSongs
Upload songs from document to database


@uploadSongs
Scenario: Upload songs
	When I send an upload catalog request
	Then I should receive a successful response
	And I should get a positive number of songs inserted
