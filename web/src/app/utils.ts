export const isFile = (item?: any): boolean => item && item.url && item.metadata?.mimeType;

export const isLink = (item?: any): boolean => item && item.url && !item.metadata?.mimeType;

export const isText = (item?: any): boolean => item && item.content;

export const isUrlValid = (url?: string): boolean => {
  if (!url) return false;

  const urlPattern = /^(?:(https?|ftp|ws):\/\/)?(?:([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}|localhost|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})(?::\d+)?(?:\/[^\s?#]*)?(?:\?[^#\s]*)?(?:#[^\s]*)?$/;
  return urlPattern.test(url);
}

export const extractErrorMessaging = (err: any): string | undefined => {
  return err.error?.message;
}

export const generatePreview = (htmlString?: string): string => {
  if (!htmlString) {
    return '';
  }

  const domParser = new DOMParser();
  const doc = domParser.parseFromString(htmlString, 'text/html');
  const text = doc.body?.textContent || '';
  return text.length > 200 ? text.substring(0, 197) + '...' : text;
}
