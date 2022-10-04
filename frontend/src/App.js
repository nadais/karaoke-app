import React, { useState, useEffect, Suspense } from 'react';
import { AgGridReact } from 'ag-grid-react';
import { useTranslation } from 'react-i18next';
import { BrowserRouter, useSearchParams } from 'react-router-dom';
import CheckboxRenderer from './CheckboxRenderer';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';

function Page() {
    const [selectedList, setSelectedList] = useState([]);
    const [songs, setSongs] = useState([]);
    const [onlyMyList, setOnlyMyList] = useState(false);
    const [searchParams, setSearchParams] = useSearchParams();
    const { t, i18n } = useTranslation();
    const [columnDefs] = useState([
        {
            headerName: translate("mylist"),
            field: "selected",
            cellRenderer: "checkboxRenderer",
            cellRendererParams:
            {
                callbackSelectedList: setSelectedList
            },
            flex: 1,
            minWidth: 80
        },
        {
            field: "number",
            headerName: translate("number"),
            sortable: true,
            filter: true,
            flex: 1,
            minWidth: 80
        },
        {
            field: "name",
            headerName: translate("name"),
            sortable: true,
            filter: true,
            flex: 4,
            minWidth: 250
        },
        {
            field: "artist",
            headerName: translate("artist"),
            sortable: true,
            filter: true,
            flex: 2,
            sort: 'asc',
            minWidth: 170
        }
    ],
    )
    const [rowData, setRowData] = useState([]);
    const [gridApi, setGridApi] = useState(null);
    const [loading, setLoading] = useState(false);
    const [catalogs, setCatalogs] = useState([]);
    const [categories, setCategories] = useState([]);
    const [genres, setGenres] = useState([]);
    const [currentGenre, setCurrentGenre] = useState(undefined);
    const [category, setCategory] = useState(undefined);
    const [catalogName, setCatalogName] = useState(undefined);
    const searchDivStyle = { padding: 10 }
    const searchStyle = {
        width: "100%", padding: "10px 20px", borderRadius: 20, outline: 0,
        border: "2px #68bf40 solid", fontSize: "100%"
    }

    const onGridReady = (params) => {
        setGridApi(params.api);
    };
    const onFilterTextChange = (e) => {
        gridApi.setQuickFilter(e.target.value)
    }
    useEffect(() => {
        let lang = searchParams.get('lang');
        if (lang != null) {
            i18n.changeLanguage(lang);
        }
        fetchSongsRemotely(lang);
    }, [i18n, searchParams]);
    function getLoading() {
        if (loading) {
            return <div className="spinner-border" role="status">
                <span className="visually-hidden"> {t('loading')}</span>
            </div>
        }
    }
    function getCategories() {
        return <div className="col">
            <select className="form-select" name='catalog' onChange={onCategoryChanged}>
                <option value=""> {t('selectCategory')}</option>
                {categories.map(key =>
                    (<option value={key} key={key}>{t(`categories.${key.toLowerCase()}`)}</option>))}
            </select>
        </div>
    }
    function getGenres() {
        return <div className="col">
            <select className="form-select" name='catalog' onChange={onGenreChanged}>
                <option value=""> {t('selectGenre')}</option>
                {genres.map(key =>
                    (<option value={key.id} key={key.id}>{key.name}</option>))}
            </select>
        </div>
    }
    function getLanguages() {
        let languages = {
            "en": "English",
            "pt": "Português",
            "fr": "Français",
            "it": "Italiano"
        }
        return <span>
            <select name='language' onChange={onLanguageChanged}>
                <option selected disabled> {t('selectLanguage')}</option>
                {Object.keys(languages).map(key =>
                    (<option value={key} key={key}>{languages[key]}</option>))}
            </select>
        </span>
    }
    function updateRowDataForMyList()
    {
        let newMyList = !onlyMyList;
        setOnlyMyList(!onlyMyList);
        let properties = getDefaultProperties();
        properties.myList = newMyList;
        setRowData(getRowData(properties));
    }
    function getMyListTrigger() {
        return <button className='btn btn-primary' onClick={updateRowDataForMyList}>
            {onlyMyList === true ?
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrow-left" viewBox="0 0 16 16">
                    <path fill-rule="evenodd" d="M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8z" />
                </svg> :
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-card-checklist" viewBox="0 0 16 16">
                    <path d="M14.5 3a.5.5 0 0 1 .5.5v9a.5.5 0 0 1-.5.5h-13a.5.5 0 0 1-.5-.5v-9a.5.5 0 0 1 .5-.5h13zm-13-1A1.5 1.5 0 0 0 0 3.5v9A1.5 1.5 0 0 0 1.5 14h13a1.5 1.5 0 0 0 1.5-1.5v-9A1.5 1.5 0 0 0 14.5 2h-13z" />
                    <path d="M7 5.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 1 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0zM7 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm-1.496-.854a.5.5 0 0 1 0 .708l-1.5 1.5a.5.5 0 0 1-.708 0l-.5-.5a.5.5 0 0 1 .708-.708l.146.147 1.146-1.147a.5.5 0 0 1 .708 0z" />
                </svg>}
        </button>
    }
    function getSearchBar() {
        if (rowData.length > 0) {
            return <div style={searchDivStyle}>
                <input type="search" style={searchStyle} onChange={onFilterTextChange} placeholder={t('searchSongs')} />
            </div>
        }
    }
    async function fetchSongsRemotely(language) {
        setLoading(true);
        let genresResponse = await fetch(`https://karaoke-api.azurewebsites.net/songs/genres?language=${language ?? ''}`);
        let result = await fetch('https://karaoke-api.azurewebsites.net/songs');
        var response = await result.json();
        var genres = await genresResponse.json();
        setGenres(genres);
        setLoading(false);
        var currentList = JSON.parse(localStorage.getItem("mylist"));
        if (currentList == null) {
            currentList = [];
        }
        setSelectedList(currentList);
        let fullCatalog = response.songGroups;
        fullCatalog = fullCatalog.map(x => {
            x.selected = currentList.includes(x.key);
            return x;
        })
        let catalogs = fullCatalog.flatMap(x => x.catalogs).filter((v, i, a) => a.indexOf(v) === i && v != null);
        let categories = fullCatalog.flatMap(x => x.categories).filter((v, i, a) => a.indexOf(v) === i && v != null);
        let availableGenres = fullCatalog
            .flatMap(x => x.genres)
            .filter((v, i, a) => a.indexOf(v) === i && v != null)
            .map(id => {
                var elem = genres.find(x => x.id === id);
                return elem;
            })
            .filter(elem => elem != null);
        setGenres(availableGenres);
        setCatalogs(catalogs);
        setCategories(categories);
        setSongs(response.songGroups);
        setRowData(fullCatalog);
    }

    const getDefaultProperties = () =>
    {
        return {
            myList: onlyMyList,
            catalog: catalogName,
            category: category,
            genre: currentGenre
        };
    }

    function getRowData(properties) {
        if (properties.myList) {
            return songs.filter(x => selectedList.includes(x.key));
        }
        let result = properties.catalog == null || properties.catalog === '' ? songs : songs.filter(x => x.catalogs.includes(properties.catalog));
        if (properties.category != null && properties.category !== '') {
            result = result.filter(x => x.categories.includes(properties.category));
        }
        if (properties.genre != null && properties.genre !== '') {
            result = result.filter(x => x.genres.includes(properties.genre));
        }
        return result;
    }

    function onCategoryChanged(event) {
        let newCategory = event.target.value === '' ? undefined : event.target.value;
        setCategory(newCategory);
        let properties = getDefaultProperties();
        properties.category = newCategory;
        setRowData(getRowData(properties));
    }
    function onGenreChanged(event) {
        let newGenre = event.target.value === '' ? undefined : parseInt(event.target.value);
        setCurrentGenre(newGenre);
        let properties = getDefaultProperties();
        properties.genre = newGenre;
        setRowData(getRowData(properties));
    }

    function onLanguageChanged(event) {
        let lang = event.target.value;
        let updatedSearchParams = new URLSearchParams(searchParams.toString());
        updatedSearchParams.set('lang', lang);
        setSearchParams(updatedSearchParams.toString());
        i18n.changeLanguage(lang);
    }

    function onCatalogChanged(event) {
        let newCatalog = event.target.value === '' ? undefined : event.target.value;
        setCatalogName(newCatalog);
        let properties = getDefaultProperties();
        properties.catalog = newCatalog;
        setRowData(getRowData(properties));
    }
    function translate(key) {
        return t(key);
    }

    return (
        <div>
            <div class="row">
                <div class="col-6 offset-md-4 offset-lg-4"><img class="header-image" src="header.png" alt="Karaoke night" /></div>
            </div>
            {getLanguages()}
            {getMyListTrigger()}
            <div className="row">
                <div className="col">
                    <select className="form-select" name='catalog' onChange={onCatalogChanged}>
                        <option value="">{t('selectCatalog')}</option>
                        {catalogs.map(key =>
                            (<option value={key} key={key}>{t(`catalogs.${key.toLowerCase()}`)}</option>))}
                    </select>
                </div>
                {getCategories()}
                {getGenres()}
                <div className="col">
                    {getLoading()}
                </div>
            </div>
            {getSearchBar()}


            <div className="ag-theme-alpine" style={{ "height": 1000 }}>
                <AgGridReact rowData={rowData}
                    defaultColDef={{
                        sortable: true,
                        resizable: true,
                        filter: true,
                        suppressMovable: true
                    }}
                    columnDefs={columnDefs}
                    localeTextFunc={translate}
                    frameworkComponents={{
                        checkboxRenderer: CheckboxRenderer
                    }}
                    onGridReady={onGridReady}>
                </AgGridReact>
            </div>
        </div>
    );
};

// here app catches the suspense from page in case translations are not yet loaded
export default function App() {
    return (
        <BrowserRouter>
            <Suspense fallback="loading">
                <Page />
            </Suspense>
        </BrowserRouter>
    );
}