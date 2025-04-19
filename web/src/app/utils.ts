export const isFile = (item? : any): boolean => item && item.url && item.metadata?.mimeType;

export const isLink = (item? : any): boolean => item && item.url && !item.metadata?.mimeType;

export const isText = (item? : any): boolean => item && item.content;

export const isUrlValid = (url? : string): boolean => {
  const urlPattern = /^(https?:\/\/)?([\w\d-]+)\.([a-z]{2,})([\/\w\d.-]*)*\/?$/;
  return urlPattern.test(url || '');
}
