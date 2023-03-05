Feature: DeleteCatalog
Delete a catalog

    @deleteCatalog
    Scenario: Delete existing catalog
        Given the following songs are loaded
          | Number | Artist      | Name       | Catalog |
          | 1      | Linkin Park | Numb       | English |
          | 2      | Linkin Park | In the End | English |
        When I send a delete songs request for English catalog
        Then I should receive a 204 response
        When I send a get songs request
        Then I should receive a 200 response
        And I should have 0 songs in response for catalog 'English'

    Scenario: Delete not existing catalog
        When I send a delete songs request for English catalog
        Then I should receive a 404 response