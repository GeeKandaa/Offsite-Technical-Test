import './App.css'
import { useEffect, useState } from 'react'
import UploadFile from './components/UploadFile'
import DataDisplay from './components/DataDisplay'

function App() {
  //#region Variables
  const [isDarkMode, setIsDarkMode] = useState(false);
  const [activeTab, setActiveTab] = useState<'upload' | 'view'>('upload');
  //#endregion

  //#region UseEffects
  /** Handle dark mode preference
   * Detects the user's preferred color scheme and handles preference change.
   * We perform this here and pass it down to components for consistency.
   */
  useEffect(() => {
    const darkModePreferred = window.matchMedia('(prefers-color-scheme: dark)');
    setIsDarkMode(darkModePreferred.matches);

    const preferenceChangeHandler = (e: MediaQueryListEvent) => setIsDarkMode(e.matches);
    darkModePreferred.addEventListener('change', preferenceChangeHandler);
    return () => {
      darkModePreferred.removeEventListener('change', preferenceChangeHandler);
    };
  }, []);
  //#endregion

  //#region Render
  return (
    <>
      <div>
        <h1>CC Technical Test Application</h1>
        <div className='tabs'>
          <button
            className={`tab-btn${activeTab === 'view' ? ' active' : ''}`}
            onClick={() => setActiveTab('upload')}
          >
            Upload File
          </button>
          <button
            className={`tab-btn${activeTab === 'view' ? ' active' : ''}`}
            onClick={() => setActiveTab('view')}
          >
            View Uploaded Data
          </button>
        </div>
        {activeTab === 'upload' && (
          <>
            <h2>Upload a file</h2>
            <UploadFile DarkMode={isDarkMode} />
          </>
        )}
        {activeTab === 'view' && (
          <>
            <h2>View Uploaded Data</h2>
            <DataDisplay DarkMode={isDarkMode} />
          </>
        )}
      </div>
    </>
  )
}
//#endregion

export default App