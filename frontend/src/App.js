import React, { useState, useEffect, Suspense } from 'react';
import { AgGridColumn, AgGridReact } from 'ag-grid-react';
import { useTranslation } from 'react-i18next';
import { BrowserRouter, useParams, useSearchParams } from 'react-router-dom';

import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';

function Page() {
    const [searchParams, setSearchParams] = useSearchParams();
    const { t, i18n } = useTranslation();
    const [rowData, setRowData] = useState([]);
    const [gridApi, setGridApi] = useState(null);
    const [loading, setLoading] = useState(false);
    const [songs, setSongs] = useState([]);
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
    }, []);
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
    function getSearchBar() {
        if (rowData.length > 0) {
            return <div style={searchDivStyle}>
                <input type="search" style={searchStyle} onChange={onFilterTextChange} placeholder={t('searchSongs')} />
            </div>
        }
    }
    async function fetchSongsRemotely(language) {
        setLoading(true);
        let genresResponse = await fetch(`https://karaoke-juliane.herokuapp.com/songs/genres?language=${language ?? ''}`);
        let result = await fetch('https://karaoke-juliane.herokuapp.com/songs');
        var response =  await result.json();
        var genres = await genresResponse.json();
        setGenres(genres);
        setLoading(false);
        let fullCatalog = response.songGroups;
        let catalogs = fullCatalog.flatMap(x => x.catalogs).filter((v, i, a) => a.indexOf(v) === i && v != null);
        let categories = fullCatalog.flatMap(x => x.categories).filter((v, i, a) => a.indexOf(v) === i && v != null);
        let availableGenres = fullCatalog
            .flatMap(x => x.genres)
            .filter((v, i, a) => a.indexOf(v) === i && v != null)
            .map(id =>
                {
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

    function getRowData(newCatalogName, newCategory, genre) {
        let result = newCatalogName == null || newCatalogName === '' ? songs : songs.filter(x => x.catalogs.includes(newCatalogName));
        if (newCategory != null && newCategory !== '') {
            result = result.filter(x => x.categories.includes(newCategory));
        }
        if (genre != null && genre !== '') {
            result = result.filter(x => x.genres.includes(genre));
        }
        return result;
    }

    function onCategoryChanged(event) {
        let newCategory = event.target.value === '' ? undefined : event.target.value;
        setCategory(newCategory);
        setRowData(getRowData(catalogName, newCategory, currentGenre));
    }
    function onGenreChanged(event) {
        let newGenre = event.target.value === '' ? undefined : parseInt(event.target.value);
        setCurrentGenre(newGenre);
        setRowData(getRowData(catalogName, category, newGenre));
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
        setRowData(getRowData(newCatalog, category, currentGenre));
    }
    function translate(key) {
        return t(key);
    }

    return (
        <div>
            <div class="row">
                <div class="col-6 offset-md-4 offset-lg-4"><img class="header-image" src="header.png" /></div>
            </div>
            {getLanguages()}
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
                    localeTextFunc={translate}
                    onGridReady={onGridReady}>
                    <AgGridColumn field="number" headerName={translate("number")} sortable={true} filter={true} flex={1} minWidth={80} />
                    <AgGridColumn field="name" headerName={translate("name")} sortable={true} filter={true} flex={4} minWidth={250} />
                    <AgGridColumn field="artist" headerName={translate("artist")} sortable={true} filter={true} flex={2} sort={'asc'} minWidth={170} />
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