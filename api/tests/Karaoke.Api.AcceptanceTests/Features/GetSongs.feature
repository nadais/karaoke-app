Feature: GetSongs
Get songs from the catalog

    @getSongs
    Scenario: Get songs
        Given the following songs are loaded
          | Number | Artist      | Name       | Catalog  |
          | 1      | Linkin Park | Numb       | GetSongs |
          | 2      | Linkin Park | In the End | GetSongs |
        When I send a get songs request
        Then I should receive a 200 response
        And I should have 2 songs in response for catalog 'GetSongs'

    Scenario: Get songs with no data
        When I send a get songs request
        Then I should receive a 200 response
        And I should have 0 songs in response for catalog 'No Response'