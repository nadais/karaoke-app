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
    const [category, setCategory] = useState(undefined);
    const [catalogName, setCatalogName] = useState(undefined);
    const searchDivStyle = { backgroundColor: "#dedede", padding: 10 }
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
        fetchSongsRemotely();
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
    function getLanguages() {
        let languages = {
            "en": "English",
            "pt": "Português"
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
    function fetchSongsRemotely() {
        setLoading(true);
        fetch('https://karaoke-juliane.herokuapp.com/songs')
            .then(result => {
                var test = result.json();
                return test;
            })
            .then(response => {
                setLoading(false);
                let fullCatalog = response.songGroups;
                let catalogs = fullCatalog.flatMap(x => x.catalogs).filter((v, i, a) => a.indexOf(v) === i && v != null);
                let categories = fullCatalog.flatMap(x => x.categories).filter((v, i, a) => a.indexOf(v) === i && v != null);
                setCatalogs(catalogs);
                setCategories(categories);
                setSongs(response.songGroups);
                setRowData(fullCatalog);
            });
    }

    function getRowData(newCatalogName, newCategory) {
        let result = newCatalogName == null || newCatalogName === '' ? songs : songs.filter(x => x.catalogs.includes(newCatalogName));
        if (newCategory != null && newCategory !== '') {
            result = result.filter(x => x.categories.includes(newCategory));
        }
        return result;
    }

    function onCategoryChanged(event) {
        let newCategory = event.target.value === '' ? undefined : event.target.value;
        setCategory(newCategory);
        setRowData(getRowData(catalogName, newCategory));
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
        setRowData(getRowData(newCatalog, category));
    }
    function translate(key) {
        return t(key);
    }

    return (
        <div>
            <div className='row'>
                <div className='col'>
                    <h1 align="center">{t('title')}</h1>
                </div>
                {getLanguages()}
            </div>
            <div className="row">
                <div className="col">
                    <select className="form-select" name='catalog' onChange={onCatalogChanged}>
                        <option value="">{t('selectCatalog')}</option>
                        {catalogs.map(key =>
                            (<option value={key} key={key}>{t(`catalogs.${key.toLowerCase()}`)}</option>))}
                    </select>
                </div>
                {getCategories()}
                <div className="col">
                    <button onClick={fetchSongsRemotely} className="btn btn-primary">
                        {t('reload')}
                    </button>
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