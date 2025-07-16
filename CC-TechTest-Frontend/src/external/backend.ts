import Config from '../Config.ts';
const DataUrl = Config.BackendUrl + '/data';
const FileUrl = Config.BackendUrl + '/file';

export const postFile = async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    const result = await fetch(FileUrl, {
        method: 'POST',
        body: formData,
    });
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const validateFile = async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    const result = await fetch(FileUrl + '/validate', {
        method: 'POST',
        body: formData,
    });
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const getAllEntries = async () => {
    const result = await fetch(DataUrl);
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const getEntryByMpan = async (mpan: string) => {
    const result = await fetch(DataUrl+`/mpan/${mpan}`);
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const getEntryBySerial = async (serial: string) => {
    const result = await fetch(DataUrl+`/serial/${serial}`);
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const getEntryByDate = async (date: string) => {
    const result = await fetch(DataUrl + `/installdate/${date}`);
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const getEntryByAddress = async (address: string) => {
    const result = await fetch(DataUrl + `/address/${address}`);
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};

export const getEntryByPostcode = async (postcode: string) => {
    const result = await fetch(DataUrl + `/postcode/${postcode}`);
    if (!result.ok) throw new Error(await result.text());
    return await result.json();
};